/*
 * MS	06-04-05	fixed sessionID on ASP.NET 2.0
 *					fixed Object.prototype.extend problem when running with third-party libs
 * MS	06-04-12	added useAssemblyQualifiedName
 * MS	06-04-25	fixed forms authentication cookieless configuration
 * MS	06-05-15	removed Class.create for JavaScript proxy
 * MS	06-05-23	using AjaxNamespace name for method
 * MS	06-06-06	fixed If-Modified-Since http header if using zip
 * MS	06-06-09	removed addNamespace use
 * 
 */
using System;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using System.Web.Caching;
using System.IO;

namespace AjaxPro
{
	/// <summary>
	/// Represents an IHttpHandler for the client-side JavaScript wrapper.
	/// </summary>
	internal class TypeJavaScriptHandler : IHttpHandler, IReadOnlySessionState	// need IReadOnlySessionState to check if using cookieless session ID
	{
		// TODO: The session ID has to be used in the cache of core and types.js
		private Type type;

		internal TypeJavaScriptHandler(Type type)
		{
			this.type = type;
		}

		#region IHttpHandler Members

		public void ProcessRequest(HttpContext context)
		{
			// The request was not a request to invoke a server-side method.
			// Now, we will render the Javascript that will be used on the
			// client to run as a proxy or wrapper for the methods marked
			// with the AjaxMethodAttribute.

			if(context.Trace.IsEnabled) context.Trace.Write(Constant.AjaxID, "Render class proxy Javascript");
		
			System.Reflection.MethodInfo[] mi = type.GetMethods();
			
			// Check wether the javascript is already rendered and cached in the
			// current context.
			
			string etag = context.Request.Headers["If-None-Match"];
			string modSince = context.Request.Headers["If-Modified-Since"];

			string path = type.FullName + "," + type.Assembly.FullName.Split(',')[0];
			if (Utility.Settings.UseAssemblyQualifiedName) path = type.AssemblyQualifiedName;

			if(Utility.Settings != null && Utility.Settings.UrlNamespaceMappings.ContainsValue(path))
			{
				foreach(string key in Utility.Settings.UrlNamespaceMappings.Keys)
				{
					if(Utility.Settings.UrlNamespaceMappings[key].ToString() == path)
					{
						path = key;
						break;
					}
				}
			}

			if(context.Cache[path] != null)
			{
				CacheInfo ci = (CacheInfo)context.Cache[path];

				if(etag != null)
				{
					if(etag == ci.ETag)		// TODO: null check
					{
						context.Response.StatusCode = 304;
						return;
					}
				}
				
				if(modSince != null)
				{
					if (modSince.IndexOf(";") > 0)
					{
						// If-Modified-Since: Tue, 06 Jun 2006 10:13:38 GMT; length=2935
						modSince = modSince.Split(';')[0];
					}

					try
					{
						DateTime modSinced = Convert.ToDateTime(modSince.ToString()).ToUniversalTime();
						if(DateTime.Compare(modSinced, ci.LastModified.ToUniversalTime()) >= 0)
						{
							context.Response.StatusCode = 304;
							return;
						}
					}
					catch(FormatException)
					{
						if(context.Trace.IsEnabled) context.Trace.Write(Constant.AjaxID, "The header value for If-Modified-Since = " + modSince + " could not be converted to a System.DateTime.");
					}
				}
			}

			etag = type.AssemblyQualifiedName; // + "_" + type.Assembly. DateTime.Now.ToString(System.Globalization.DateTimeFormatInfo.CurrentInfo.SortableDateTimePattern);
			etag = MD5Helper.GetHash(System.Text.Encoding.Default.GetBytes(etag));

			DateTime now = DateTime.Now;
			DateTime lastMod = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second); // .ToUniversalTime();

			context.Response.AddHeader("Content-Type", "application/x-javascript");
			context.Response.ContentEncoding = System.Text.Encoding.UTF8;
			context.Response.Cache.SetCacheability(System.Web.HttpCacheability.Public);
			context.Response.Cache.SetETag(etag);
			context.Response.Cache.SetLastModified(lastMod);

			// Ok, we do not have the javascript rendered, yet.
			// Build the javascript source and save it to the current
			// Application context.

			System.Text.StringBuilder sb = new System.Text.StringBuilder();


			AjaxNamespaceAttribute[] cma = (AjaxNamespaceAttribute[])type.GetCustomAttributes(typeof(AjaxNamespaceAttribute), true);
			string clientNS = type.FullName;

			if(cma.Length > 0 && cma[0].ClientNamespace != null)
			{
				//if (cma[0].ClientNamespace.IndexOf(".") > 0)
				//  sb.Append("addNamespace(\"" + cma[0].ClientNamespace + "\");\r\n");

				clientNS = cma[0].ClientNamespace;
			}
			else
			{
				//sb.Append("addNamespace(\"" + (type.FullName.IndexOf(".") > 0 ? type.FullName.Substring(0, type.FullName.LastIndexOf(".")) : type.FullName) + "\");\r\n");
			}

			sb.Append(JavaScriptUtil.GetClientNamespaceRepresentation(clientNS));

			sb.Append(clientNS);
			sb.Append("_class = function() {};\r\n");
			

			sb.Append("Object.extend(");
			sb.Append(clientNS);
			sb.Append("_class.prototype, Object.extend(new AjaxPro.AjaxClass(), {\r\n");

			System.Reflection.MethodInfo method;

			for(int y=0; y<mi.Length; y++)
			{
				method = mi[y];

				if(!method.IsPublic)
					continue;

				AjaxNamespaceAttribute[] cmam = (AjaxNamespaceAttribute[])method.GetCustomAttributes(typeof(AjaxNamespaceAttribute), true);
				AjaxMethodAttribute[] ma = (AjaxMethodAttribute[])method.GetCustomAttributes(typeof(AjaxMethodAttribute), true);

				if(ma.Length == 0)
					continue;

				System.Reflection.ParameterInfo[] pi = method.GetParameters();

				// Render the function header

				sb.Append("\t");

				if(cmam.Length == 0)
					sb.Append(method.Name);
				else
					sb.Append(cmam[0].ClientNamespace);

				sb.Append(": function(");


				// Render all parameters

				for(int i=0; i<pi.Length; i++)
				{
					sb.Append(pi[i].Name);

					if(i<pi.Length -1)
						sb.Append(", ");
				}
	
				sb.Append(") {\r\n");


				// Create the XMLHttpRequest object

				sb.Append("\t\treturn this.invoke(\"");

				if (cmam.Length == 0)
					sb.Append(method.Name);
				else
					sb.Append(cmam[0].ClientNamespace);

				sb.Append("\", {");

				for(int i=0; i<pi.Length; i++)
				{
					sb.Append("\"");
					sb.Append(pi[i].Name);
					sb.Append("\":");
					sb.Append(pi[i].Name);

					if(i<pi.Length -1)
						sb.Append(", ");
				}

				sb.Append("}, this.");
				
				if(cmam.Length == 0)
					sb.Append(method.Name);
				else
					sb.Append(cmam[0].ClientNamespace);

				sb.Append(".getArguments().slice(");
				sb.Append(pi.Length.ToString());
				sb.Append("));\r\n\t},\r\n");
			}

			sb.Append("\turl: '");
			string url = context.Request.ApplicationPath + (context.Request.ApplicationPath.EndsWith("/") ? "" : "/") + Utility.HandlerPath + "/" + AjaxPro.Utility.GetSessionUri() + path + Utility.HandlerExtension;
			sb.Append(url);
			sb.Append("'\r\n");


			sb.Append("}));\r\n");


			sb.Append(clientNS);
			sb.Append(" = new ");
			sb.Append(clientNS);
			sb.Append("_class();\r\n");

			context.Response.Write(sb.ToString());
			context.Response.Write("\r\n");


			// save the javascript in current Application context to
			// speed up the next requests.

			// TODO: was können wir hier machen??
			// System.Web.Caching.CacheDependency fileDepend = new System.Web.Caching.CacheDependency(type.Assembly.Location);

			context.Cache.Add(path, new CacheInfo(etag, lastMod), null,
				System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration,
				System.Web.Caching.CacheItemPriority.Normal, null);

			if(context.Trace.IsEnabled) context.Trace.Write(Constant.AjaxID, "End ProcessRequest");
		}

		public bool IsReusable
		{
			get
			{
				// TODO:  Add CoreJavaScriptHandler.IsReusable getter implementation
				return false;
			}
		}

		#endregion
	}
}

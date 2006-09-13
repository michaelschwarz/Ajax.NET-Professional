/*
 * MS	06-05-30	changed using new converter collections
 * MS	06-06-06	fixed If-Modified-Since http header if using zip
 * MS	06-06-07	changed to internal
 * 
 * 
 */
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.IO;

namespace AjaxPro
{
	/// <summary>
	/// Represents an IHttpHandler for the client-side JavaScript converter methods.
	/// </summary>
	internal class ConverterJavaScriptHandler : IHttpHandler
	{
		#region IHttpHandler Members

		public void ProcessRequest(HttpContext context)
		{
			string etag = context.Request.Headers["If-None-Match"];
			string modSince = context.Request.Headers["If-Modified-Since"];

			if(context.Cache[Constant.AjaxID + ".converter"] != null)
			{
				CacheInfo ci = (CacheInfo)context.Cache[Constant.AjaxID + ".converter"];

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

			etag = MD5Helper.GetHash(System.Text.Encoding.Default.GetBytes("converter"));

			DateTime now = DateTime.Now;
			DateTime lastMod = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second); //.ToUniversalTime();

			context.Response.AddHeader("Content-Type", "application/x-javascript");
			context.Response.ContentEncoding = System.Text.Encoding.UTF8;
			context.Response.Cache.SetCacheability(System.Web.HttpCacheability.Public);
			context.Response.Cache.SetETag(etag);
			context.Response.Cache.SetLastModified(lastMod);



			if(Utility.Settings != null && Utility.Settings.Encryption != null)
			{
				context.Response.Write(Utility.Settings.Encryption.CryptProvider.ClientScript);
				context.Response.Write("\r\n");

				context.Response.Write(Utility.Settings.Encryption.KeyProvider.ClientScript);
				context.Response.Write("\r\n");
			}

			StringCollection convTypes = new StringCollection();
			string convTypeName;
			IJavaScriptConverter c;
			string script;

			IEnumerator s = Utility.Settings.SerializableConverters.Values.GetEnumerator();
			while(s.MoveNext())
			{
				c = (IJavaScriptConverter)s.Current;
				convTypeName = c.GetType().FullName;
				if (!convTypes.Contains(convTypeName))
				{
					script = c.GetClientScript();

					if (script.Length > 0)
					{
#if(NET20)
						if(!String.IsNullOrEmpty(c.ConverterName))
#else
						if(c.ConverterName != null && c.ConverterName.Length > 0)
#endif
							context.Response.Write("// " + c.ConverterName + "\r\n");
						context.Response.Write(c.GetClientScript());
						context.Response.Write("\r\n");
					}

					convTypes.Add(convTypeName);
				}
			}

			IEnumerator d = Utility.Settings.DeserializableConverters.Values.GetEnumerator();
			while(d.MoveNext())
			{
				c = (IJavaScriptConverter)d.Current;
				convTypeName = c.GetType().FullName;
				if (!convTypes.Contains(convTypeName))
				{
					script = c.GetClientScript();

					if (script.Length > 0)
					{
#if(NET20)
						if (!String.IsNullOrEmpty(c.ConverterName))
#else
						if(c.ConverterName != null && c.ConverterName.Length > 0)
#endif

							context.Response.Write("// " + c.ConverterName + "\r\n");
						context.Response.Write(c.GetClientScript());
						context.Response.Write("\r\n");
					}

					convTypes.Add(convTypeName);
				}
			}

			context.Cache.Add(Constant.AjaxID + ".converter", new CacheInfo(etag, lastMod), null, 
				System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration,
				System.Web.Caching.CacheItemPriority.Normal, null);
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		#endregion
	}
}

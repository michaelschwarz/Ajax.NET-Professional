/*
 * MS	06-04-05	added oldstyled Object.prototype.extend code, enabled by web.config
 *					setting oldStyle\objectExtendPrototype
 * MS	06-05-22	added possibility to have one file for prototype,core instead of two
 * MS	06-06-06	fixed If-Modified-Since http header if using zip
 * MS	06-06-07	changed to internal
 * 
 * 
 */
using System;
using System.Reflection;
using System.Web;
using System.Web.Caching;
using System.IO;

namespace AjaxPro
{
	/// <summary>
	/// Represents an IHttpHandler for the client-side JavaScript prototype and core methods.
	/// </summary>
	internal class EmbeddedJavaScriptHandler : IHttpHandler
	{
		private string fileName;

		internal EmbeddedJavaScriptHandler(string fileName)
		{
			this.fileName = fileName;
		}

		#region IHttpHandler Members

		public void ProcessRequest(HttpContext context)
		{
			string etag = context.Request.Headers["If-None-Match"];
			string modSince = context.Request.Headers["If-Modified-Since"];

			if(context.Cache[Constant.AjaxID + "." + fileName] != null)
			{
				CacheInfo ci = (CacheInfo)context.Cache[Constant.AjaxID + "." + fileName];

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

			etag = MD5Helper.GetHash(System.Text.Encoding.Default.GetBytes(fileName));

			DateTime now = DateTime.Now;
			DateTime lastMod = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second); //.ToUniversalTime();

			context.Response.AddHeader("Content-Type", "application/x-javascript");
			context.Response.ContentEncoding = System.Text.Encoding.UTF8;
			context.Response.Cache.SetCacheability(System.Web.HttpCacheability.Public);
			context.Response.Cache.SetETag(etag);
			context.Response.Cache.SetLastModified(lastMod);


			// Now, we want to read the JavaScript embedded source
			// from the assembly. If the filename includes any comma
			// we have to return more than one embedded JavaScript file.

			string[] files = fileName.Split(',');
			Assembly assembly = Assembly.GetExecutingAssembly();
			Stream s;

			for (int i = 0; i < files.Length; i++)
			{
				s = assembly.GetManifestResourceStream(Constant.AssemblyName + "." + files[i] + ".js");

				if (s != null)
				{
					System.IO.StreamReader sr = new System.IO.StreamReader(s);

					context.Response.Write(sr.ReadToEnd());
					context.Response.Write("\r\n");

					sr.Close();

					if (files[i] == "prototype")
					{
						if (AjaxPro.Utility.Settings.OldStyle.Contains("objectExtendPrototype"))
						{
							context.Response.Write(@"
Object.prototype.extend = function(o, override) {
	return Object.extend.apply(this, [this, o, override != false]);
}
");
						}
					}
				}
			}

			context.Cache.Add(Constant.AjaxID + "." + fileName, new CacheInfo(etag, lastMod), null, 
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

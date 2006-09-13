#if(NET20)
/*
 * MS	06-06-06	initial version (ASP.NET 2.0 only)
 * 
 * 
 */
using System;
using System.IO;
using System.Text;
using System.Web;
using System.IO.Compression;

namespace AjaxPro
{
	public class HttpCompressionModule : IHttpModule
	{
		public HttpCompressionModule()
		{
		}

		#region IHttpModule Members

		void IHttpModule.Dispose()
		{

		}

		void IHttpModule.Init(HttpApplication context)
		{
			context.BeginRequest += (new EventHandler(this.context_BeginRequest));
		}

		#endregion

		void context_BeginRequest(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;
			string encodings = app.Request.Headers.Get("Accept-Encoding");

			if (encodings == null)
				return;

			string url = app.Request.RawUrl.ToLower();

			if (!url.StartsWith((app.Request.ApplicationPath == "/" ? app.Request.ApplicationPath : app.Request.ApplicationPath + "/")
				+ Utility.HandlerPath))
				return;

			Stream baseStream = app.Response.Filter;
			encodings = encodings.ToLower();

			if (encodings.Contains("gzip"))
			{
				app.Response.Filter = new GZipStream(baseStream, CompressionMode.Compress);
				app.Response.AppendHeader("Content-Encoding", "gzip");
			}
			else if (encodings.Contains("deflate"))
			{
				app.Response.Filter = new DeflateStream(baseStream, CompressionMode.Compress);
				app.Response.AppendHeader("Content-Encoding", "deflate");
			}
		}
	}
}
#endif
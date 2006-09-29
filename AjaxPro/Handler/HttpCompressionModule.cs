/*
 * HttpCompressionModule.cs
 * 
 * Copyright © 2006 Michael Schwarz (http://www.ajaxpro.info).
 * All Rights Reserved.
 * 
 * Permission is hereby granted, free of charge, to any person 
 * obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without 
 * restriction, including without limitation the rights to use, 
 * copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be 
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR 
 * ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
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
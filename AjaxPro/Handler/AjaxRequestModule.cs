/*
 * MS	06-06-12	initial version
 * 
 * 
 */
using System;
using System.IO;
using System.Text;
using System.Web;

namespace AjaxPro
{
	public class AjaxRequestModule : IHttpModule
	{
		public AjaxRequestModule()
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
			
			string url = app.Request.RawUrl.ToLower();

			if (!url.StartsWith((app.Request.ApplicationPath == "/" ? app.Request.ApplicationPath : app.Request.ApplicationPath + "/")
				+ Utility.HandlerPath))
				return;

			if (app.Request.UrlReferrer == null || app.Context.Request.UrlReferrer.Host != app.Context.Request.Url.Host)
			{
				throw new HttpException(500, "Access denied.");
			}
		}
	}
}
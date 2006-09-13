/*
 * MS	06-06-07	changed to internal
 * 
 * 
 * 
 */
using System;
using System.Reflection;
using System.Web;
using System.Threading;
using System.Web.SessionState;

namespace AjaxPro
{
	internal class AjaxSyncHttpHandler : IHttpHandler
	{
		private IAjaxProcessor p;

		internal AjaxSyncHttpHandler(IAjaxProcessor p)
			: base()
		{
			this.p = p;
		}

		#region IHttpHandler Members

		public void ProcessRequest(HttpContext context)
		{
			AjaxProcHelper m = new AjaxProcHelper(p);
			m.Run();
		}

		public bool IsReusable
		{
			get
			{
				// TODO:  Add AjaxAsyncHttpHandler.IsReusable getter implementation
				return false;
			}
		}

		#endregion
	}


	internal class AjaxSyncHttpHandlerSession : AjaxSyncHttpHandler, IRequiresSessionState
	{
		internal AjaxSyncHttpHandlerSession(IAjaxProcessor p) : base(p) { }
	}

	internal class AjaxSyncHttpHandlerSessionReadOnly : AjaxSyncHttpHandler, IReadOnlySessionState
	{
		internal AjaxSyncHttpHandlerSessionReadOnly(IAjaxProcessor p) : base(p) { }
	}
}

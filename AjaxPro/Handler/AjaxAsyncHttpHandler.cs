/*
 * MS	06-06-07	changed to internal
 * MS	06-06-11	changed to use IAsyncResult and BeginInvoke instead of new Thread
 * 
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
	internal delegate void AsyncAjaxProcDelegate();

	internal class AjaxAsyncHttpHandler : IHttpAsyncHandler
	{
		private IAjaxProcessor p;
		private HttpContext ctx = null;

		internal AjaxAsyncHttpHandler(IAjaxProcessor p)
			: base()
		{
			this.p = p;
		}

		#region IHttpAsyncHandler Members

		public IAsyncResult BeginProcessRequest(HttpContext context, System.AsyncCallback cb, object extraData)
		{
			ctx = context;
			IntPtr token = System.Security.Principal.WindowsIdentity.GetCurrent().Token;

			AjaxProcHelper m = new AjaxProcHelper(p, token);
			AsyncAjaxProcDelegate apd = new AsyncAjaxProcDelegate(m.Run);

			IAsyncResult ar = apd.BeginInvoke(cb, apd);

			return ar;
		}

		public void EndProcessRequest(IAsyncResult ar)
		{
			AsyncAjaxProcDelegate apd = (AsyncAjaxProcDelegate)ar.AsyncState;

			apd.EndInvoke(ar);

			//AjaxProcHelper m = (AjaxProcHelper)apd.Target;
		}

		#endregion

		#region IHttpHandler Members

		public void ProcessRequest(HttpContext context)
		{
			// TODO:  Add AjaxAsyncHttpHandler.ProcessRequest implementation
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


	internal class AjaxAsyncHttpHandlerSession : AjaxAsyncHttpHandler, IRequiresSessionState
	{
		internal AjaxAsyncHttpHandlerSession(IAjaxProcessor p) : base(p) { }
	}

	internal class AjaxAsyncHttpHandlerSessionReadOnly : AjaxAsyncHttpHandler, IReadOnlySessionState
	{
		internal AjaxAsyncHttpHandlerSessionReadOnly(IAjaxProcessor p) : base(p) { }
	}
}

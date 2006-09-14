#if(NET20)
/*
 * MS	06-05-22	inital version
 * MS	06-09-14	mark for NET20 only
 * 
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web.UI.Design;
using System.Reflection;

namespace AjaxPro.Web.UI
{
	public class AjaxBasePage : System.Web.UI.Page, AjaxPro.IContextInitializer
	{
		protected HttpContext m_HttpContext;

		public AjaxBasePage()
			: base()
		{
			this.Load += new EventHandler(AjaxBasePage_Load);
		}

		void AjaxBasePage_Load(object sender, EventArgs e)
		{
		}

		protected new System.Web.Caching.Cache Cache
		{
			get
			{
				return m_HttpContext.Cache;
			}
		}

		protected override HttpContext Context
		{
			get
			{
				return m_HttpContext;
			}
		}

		protected new System.Web.SessionState.HttpSessionState Session
		{
			get
			{
				return m_HttpContext.Session;
			}
		}

		#region IContextInitializer Member

		public void InitializeContext(HttpContext context)
		{
			m_HttpContext = context;
		}

		#endregion
	}
}
#endif
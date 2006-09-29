/*
 * AjaxBasePage.cs
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
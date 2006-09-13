/*
 * MS	06-05-03	initial version (thanks to Albert)
 * MS	06-05-09	added new constructor
 * 
 */
#if(WEBEVENT)
using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Management;
using System.Reflection;
using System.Web;
using AjaxPro;

namespace AjaxPro.Management
{
    public class WebAjaxErrorEvent : WebBaseErrorEvent
    {
        IAjaxProcessor ajaxProcessor;
        object[] parameters;
        Exception exception;

		public WebAjaxErrorEvent(string message, int eventCode, Exception exception)
			: base(message, null, eventCode, exception)
		{
			this.exception = exception;
		}

		public WebAjaxErrorEvent(string message, IAjaxProcessor ajaxProcessor, int eventCode, Exception exception)
			: base(message, ajaxProcessor, eventCode, exception)
		{
			this.ajaxProcessor = ajaxProcessor;
			this.exception = exception;
		}

		public WebAjaxErrorEvent(string message, IAjaxProcessor ajaxProcessor, object[] parameters, int eventCode, Exception exception) 
			: base(message, ajaxProcessor, eventCode, exception)
        {
            this.ajaxProcessor = ajaxProcessor;
            this.parameters = parameters;
            this.exception = exception;
        }

        public override void FormatCustomEventDetails(WebEventFormatter formatter)
        {
			if (ajaxProcessor != null)
			{
				MethodInfo methodInfo = ajaxProcessor.AjaxMethod;
				object[] paremeters = ajaxProcessor.RetreiveParameters();
				string str = string.Format("TargetSite: {0}", exception.TargetSite);

				formatter.AppendLine(str);
				formatter.AppendLine("");

				if (System.Web.HttpContext.Current.Items[Constant.AjaxID + ".JSON"] != null)
				{
					formatter.AppendLine("JSON: " + System.Web.HttpContext.Current.Items[Constant.AjaxID + ".JSON"].ToString());
					formatter.AppendLine("");
				}

				str = string.Format(Constant.AjaxID + " Method: {0}.{1}", methodInfo.ReflectedType.ToString(), methodInfo.Name);

				formatter.AppendLine(str);
				formatter.AppendLine("");

				if (this.parameters != null)
				{
					foreach (object o in this.parameters)
					{
						formatter.AppendLine(string.Format("Parameter: {0}", o));
					}

					formatter.AppendLine("");
				}

				formatter.AppendLine("StrackTrace:");
				formatter.AppendLine(exception.StackTrace);
			}
			else
			{
				string str = string.Format("TargetSite: {0}", exception.TargetSite);

				formatter.AppendLine(str);
				formatter.AppendLine("");

				if (System.Web.HttpContext.Current.Items[Constant.AjaxID + ".JSON"] != null)
				{
					formatter.AppendLine("JSON: " + System.Web.HttpContext.Current.Items[Constant.AjaxID + ".JSON"].ToString());
					formatter.AppendLine("");
				}

				formatter.AppendLine("StrackTrace:");
				formatter.AppendLine(exception.StackTrace);
			}

            base.FormatCustomEventDetails(formatter);
        }
    }
}
#endif
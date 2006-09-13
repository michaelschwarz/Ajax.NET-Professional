/*
 * MS	06-05-24	initial version
 * 
 */
using System;
using System.Text;
using System.Data;

namespace AjaxPro
{
	/// <summary>
	/// Provides methods to serialize and deserialize a Exception object.
	/// </summary>
	public class ExceptionConverter : IJavaScriptConverter
	{
		public ExceptionConverter()
			: base()
		{
			m_AllowInheritance = true;

			m_serializableTypes = new Type[] {
				typeof(Exception),
				typeof(NotImplementedException),
				typeof(NotSupportedException),
				typeof(NullReferenceException),
				typeof(System.Security.SecurityException)
			};
		}
	
		public override string Serialize(object o)
		{
			Exception ex = (Exception)o;
			StringBuilder sb = new StringBuilder();

			// The following line is NON-JSON format, it is used to 
			// return null to res.value and have an additional property res.error
			// in the object the callback JavaScript method will get.

			sb.Append("null; r.error = ");
			
			
			sb.Append("{\"Message\":");
			sb.Append(JavaScriptSerializer.Serialize(ex.Message));
			sb.Append(",\"Type\":");
			sb.Append(JavaScriptSerializer.Serialize(o.GetType().FullName));
#if(!JSONLIB)
			if (AjaxPro.Utility.Settings.DebugEnabled)
			{
				sb.Append(",\"Stack\":");
				sb.Append(JavaScriptSerializer.Serialize(ex.StackTrace));

				if (ex.TargetSite != null)
				{
					sb.Append(",\"TargetSite\":");
					sb.Append(JavaScriptSerializer.Serialize(ex.TargetSite.ToString()));
				}

				sb.Append(",\"Source\":");
				sb.Append(JavaScriptSerializer.Serialize(ex.Source));
			}
#endif
			sb.Append("}");

			return sb.ToString();
		}

		public override bool TrySerializeValue(object o, Type t, out string json)
		{
			Exception ex = o as Exception;
			if (ex != null)
			{
				json = this.Serialize(ex);
				return true;
			}

			return base.TrySerializeValue(o, t, out json);
		}
	}
}

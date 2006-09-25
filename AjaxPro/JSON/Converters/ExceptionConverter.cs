/*
 * MS	06-05-24	initial version
 * MS	06-09-24	use QuoteString instead of Serialize
 * MS	06-09-26	improved performance using StringBuilder
 * 
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
			StringBuilder sb = new StringBuilder();
			Serialize(o, sb);
			return sb.ToString();
		}
	
		public override void Serialize(object o, StringBuilder sb)
		{
			Exception ex = (Exception)o;

			// The following line is NON-JSON format, it is used to 
			// return null to res.value and have an additional property res.error
			// in the object the callback JavaScript method will get.

			sb.Append("null; r.error = ");
			
			
			sb.Append("{\"Message\":");
			JavaScriptUtil.QuoteString(ex.Message, sb);
			sb.Append(",\"Type\":");
			JavaScriptUtil.QuoteString(o.GetType().FullName, sb);
#if(!JSONLIB)
			if (AjaxPro.Utility.Settings.DebugEnabled)
			{
				sb.Append(",\"Stack\":");
				JavaScriptUtil.QuoteString(ex.StackTrace, sb);

				if (ex.TargetSite != null)
				{
					sb.Append(",\"TargetSite\":");
					JavaScriptUtil.QuoteString(ex.TargetSite.ToString(), sb);
				}

				sb.Append(",\"Source\":");
				JavaScriptUtil.QuoteString(ex.Source, sb);
			}
#endif
			sb.Append("}");
		}

		public override bool TrySerializeValue(object o, Type t, StringBuilder sb)
		{
			Exception ex = o as Exception;
			if (ex != null)
			{
				this.Serialize(ex, sb);
				return true;
			}

			return base.TrySerializeValue(o, t, sb);
		}
	}
}

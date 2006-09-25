/*
 * MS	06-05-24	initial version
 * MS	06-09-26	improved performance using StringBuilder
 * 
 * 
 * 
 */
using System;
using System.Text;
using System.Data;

namespace AjaxPro
{
	/// <summary>
	/// Provides methods to serialize and deserialize a Guid object.
	/// </summary>
	public class GuidConverter : IJavaScriptConverter
	{
		public GuidConverter()
			: base()
		{
			m_serializableTypes = new Type[] {
				typeof(Guid)
			};
			m_deserializableTypes = m_serializableTypes;
		}

		public override string Serialize(object o)
		{
			StringBuilder sb = new StringBuilder();
			Serialize(o, sb);
			return sb.ToString();
		}

		public override void Serialize(object o, StringBuilder sb)
		{
			// Guids are not supported by JavaScript, we will return the
			// string representation using following format:
			// xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx 

			JavaScriptUtil.QuoteString(o.ToString(), sb);
		}

		public override object Deserialize(IJavaScriptObject o, Type t)
		{
			return new Guid(o.ToString());
		}
	}
}

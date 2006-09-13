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
			// Guids are not supported by JavaScript, we will return the
			// string representation using following format:
			// xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx 

			return JavaScriptSerializer.Serialize(o.ToString());
		}

		public override object Deserialize(IJavaScriptObject o, Type t)
		{
			return new Guid(o.ToString());
		}
	}
}

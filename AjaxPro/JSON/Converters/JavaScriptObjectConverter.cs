/*
 * MS	06-04-29	initial version
 * MS	06-05-23	using local variables instead of "new Type()" for get De-/SerializableTypes
 * MS	06-07-19	fixed if method argument is from type IJavaScriptObject, now done in IAjaxProcessor
 * MS	06-07-20	added missing deserialize method
 * MS	06-09-24	use QuoteString instead of Serialize
 * MS	06-09-26	improved performance using StringBuilder
 * 
 * 
 */
using System;
using System.Text;

namespace AjaxPro
{
	/// <summary>
	/// Provides methods to serialize and deserialize a JavaScriptObject object.
	/// </summary>
	public class IJavaScriptObjectConverter : IJavaScriptConverter
	{
		public IJavaScriptObjectConverter()
			: base()
		{
			m_serializableTypes = new Type[] {
				typeof(IJavaScriptObject),
				typeof(JavaScriptArray),
				typeof(JavaScriptBoolean),
				typeof(JavaScriptNumber),
				typeof(JavaScriptObject),
				typeof(JavaScriptString),
				typeof(JavaScriptSource)
			};

			m_deserializableTypes = m_serializableTypes;
		}

		public override object Deserialize(IJavaScriptObject o, Type t)
		{
			return o;
		}

		public override string Serialize(object o)
		{
			StringBuilder sb = new StringBuilder();
			Serialize(o, sb);
			return sb.ToString();
		}

		public override void Serialize(object o, StringBuilder sb)
		{
			JavaScriptObject j = o as JavaScriptObject;

			if (j == null)
			{
				sb.Append(((IJavaScriptObject)o).Value);
				return;
			}

			bool b = true;

			sb.Append("{");

			foreach (string key in j.Keys)
			{
				if(b){ b = false; }
				else{ sb.Append(","); }

				JavaScriptUtil.QuoteString(key, sb);
				sb.Append(":");

				sb.Append(JavaScriptSerializer.Serialize((IJavaScriptObject)j[key]));
			}

			sb.Append("}");
		}

		//public override bool TryDeserializeValue(IJavaScriptObject jso, Type t, out object o)
		//{
		//    if (typeof(IJavaScriptObject).IsAssignableFrom(t))
		//    {
		//        o = jso;
		//        return true;
		//    }

		//    return base.TryDeserializeValue(jso, t, out o);
		//}
	}
}

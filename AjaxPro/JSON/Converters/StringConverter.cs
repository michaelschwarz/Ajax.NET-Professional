/*
 * MS	06-05-24	initial version
 *					allowNumberBooleanAsString
 * 
 */
using System;
using System.Text;
using System.Data;

namespace AjaxPro
{
	/// <summary>
	/// Provides methods to serialize and deserialize a String object.
	/// </summary>
	public class StringConverter : IJavaScriptConverter
	{
		public StringConverter()
			: base()
		{
			m_serializableTypes = new Type[] {
				typeof(String), 
				typeof(Char)
			};
			m_deserializableTypes = m_serializableTypes;
		}
	
		public override string Serialize(object o)
		{
			return JavaScriptUtil.QuoteString(o.ToString());
		}

		public override object Deserialize(IJavaScriptObject o, Type t)
		{
#if(!JSONLIB)
			if (!Utility.Settings.OldStyle.Contains("allowNumberBooleanAsString"))
#endif
			{
				if (o is JavaScriptNumber)
					return JavaScriptDeserializer.Deserialize(o, typeof(Int64));
				else if (o is JavaScriptBoolean)
					return JavaScriptDeserializer.Deserialize(o, typeof(Boolean));
			}

			if (t == typeof(char))
			{
				string s = o.ToString();

				if (s.Length == 0)
					return '\0';
				return s[0];
			}

			return o.ToString();
		}

		public override bool TryDeserializeValue(IJavaScriptObject jso, Type t, out object o)
		{
			if (t.IsAssignableFrom(typeof(string)))
			{
				o = this.Deserialize(jso, t);
				return true;
			}

			return base.TryDeserializeValue(jso, t, out o);
		}
	}
}

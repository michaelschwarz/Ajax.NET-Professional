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
	/// Provides methods to serialize and deserialize a Enum object.
	/// </summary>
	public class EnumConverter : IJavaScriptConverter
	{
		public EnumConverter()
			: base()
		{
			m_serializableTypes = new Type[] {
				typeof(Enum)
			};
			m_deserializableTypes = m_serializableTypes;
		}
	
		public override string Serialize(object o)
		{
			// this.SerializeValue(Decimal.GetBits(d), sb);

			// The following code is not correct, but while JavaScript cannot
			// handle the decimal value correct we are using double instead.

			return JavaScriptSerializer.Serialize((Double)o);
		}

		public override object Deserialize(IJavaScriptObject o, Type t)
		{
			//return Enum.Parse(type, o.ToString());

			object enumValue = JavaScriptDeserializer.Deserialize(o, Enum.GetUnderlyingType(t));

			if (!Enum.IsDefined(t, enumValue))
				throw new ArgumentException("Invalid value for enum of type '" + t.ToString() + "'.", "o");

			return Enum.ToObject(t, enumValue);
		}

		public override bool TrySerializeValue(object o, Type t, out string json)
		{
			if (t.IsEnum)
			{
				json = JavaScriptSerializer.Serialize(Convert.ChangeType(o, Enum.GetUnderlyingType(t)));
				return true;
			}

			return base.TrySerializeValue(o, t, out json);
		}

		public override bool TryDeserializeValue(IJavaScriptObject jso, Type t, out object o)
		{
			if (t.IsEnum)
			{
				o = this.Deserialize(jso, t);
				return true;
			}

			return base.TryDeserializeValue(jso, t, out o);
		}
	}
}

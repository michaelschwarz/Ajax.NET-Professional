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
	/// Provides methods to serialize and deserialize a primitive object.
	/// </summary>
	public class PrimitiveConverter : IJavaScriptConverter
	{
		public PrimitiveConverter()
			: base()
		{
			m_serializableTypes = new Type[] {
				typeof(Boolean),
				typeof(Byte), typeof(SByte),
				typeof(Int16), typeof(UInt16), typeof(Int32), typeof(UInt32), typeof(Int64), typeof(UInt64),
				// typeof(Char),		// Char is handeled by StringConverter
				typeof(Double), typeof(Single)
			};

			m_deserializableTypes = m_serializableTypes;
		}
	
		public override string Serialize(object o)
		{
			if (o is Boolean)
			{
				return ((bool)o == true ? "true" : "false");
			}

			if (o is Byte || o is SByte)
			{
				return o.ToString();
			}

			if (o is Single && Single.IsNaN((Single)o))
			{
				throw new ArgumentException("object must be a valid number (float.NaN)", "o");
			}

			if (o is Double && Double.IsNaN((Double)o))
			{
				throw new ArgumentException("object must be a valid number (double.NaN)", "o");
			}

			// Shave off trailing zeros and decimal point, if possible
			string s = o.ToString().ToLower().Replace(System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator, ".");

			if (s.IndexOf('e') < 0 && s.IndexOf('.') > 0)
			{
				while (s.EndsWith("0"))
				{
					s.Substring(0, s.Length - 1);
				}
				if (s.EndsWith("."))
				{
					s.Substring(0, s.Length - 1);
				}
			}

			return s;
		}

		public override object Deserialize(IJavaScriptObject o, Type t)
		{
			if (!t.IsPrimitive)
				throw new NotSupportedException();

			string s = o.ToString();

			// TODO: return the default value for this primitive data type
			s = s.Replace(".", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);

			if (t == typeof(Boolean) && o is JavaScriptBoolean)
				return (bool)((JavaScriptBoolean)o);

			if (t == typeof(Byte))
				return Byte.Parse(s);
			else if (t == typeof(SByte))
				return SByte.Parse(s);
			else if (t == typeof(Int16))
				return Int16.Parse(s);
			else if (t == typeof(UInt16))
				return UInt16.Parse(s);
			else if (t == typeof(Int32))
				return Int32.Parse(s);
			else if (t == typeof(UInt32))
				return UInt32.Parse(s);
			else if (t == typeof(Int64))
				return Int64.Parse(s);
			else if (t == typeof(UInt64))
				return UInt64.Parse(s);
			else if (t == typeof(Double))
				return Double.Parse(s);
			else if (t == typeof(Single))
				return Single.Parse(s);

			throw new NotImplementedException("This primitive data type '" + t.FullName + "' is not implemented.");
		}
	}
}

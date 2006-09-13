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
	/// Provides methods to serialize and deserialize a Decimal object.
	/// </summary>
	public class DecimalConverter : IJavaScriptConverter
	{
		public DecimalConverter()
			: base()
		{
			m_serializableTypes = new Type[] {
				typeof(Decimal)
			};
			m_deserializableTypes = m_serializableTypes;
		}
	
		public override string Serialize(object o)
		{
			// this.SerializeValue(Decimal.GetBits(d), sb);

			// The following code is not correct, but while JavaScript cannot
			// handle the decimal value correct we are using double instead.

			return JavaScriptSerializer.Serialize((Double)((Decimal)o));
		}

		public override object Deserialize(IJavaScriptObject o, Type t)
		{
			JavaScriptNumber n = o as JavaScriptNumber;

			if (n == null)
				throw new NotSupportedException();

			string s =n.Value;

			// TODO: return the default value for this primitive data type
			s = s.Replace(".", System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator);

			Decimal d = Decimal.Parse(s,
				System.Globalization.NumberStyles.Currency |
				System.Globalization.NumberStyles.AllowExponent);

			return d;
		}
	}
}

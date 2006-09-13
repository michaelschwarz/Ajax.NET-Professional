/*
 * MS	06-04-26	added implicit operators for short,int,long
 * MS	06-05-31	added cstor for primitive types
 * 
 * 
 */
using System;
using System.Collections;

namespace AjaxPro
{
	/// <summary>
	/// Represents a JavaScript ECMA number.
	/// </summary>
	public class JavaScriptNumber : IJavaScriptObject
	{
		private string _value = string.Empty;

		/// <summary>
		/// Initializes a new JavaScript number instance.
		/// </summary>
		public JavaScriptNumber()
			: base()
		{
		}

		public JavaScriptNumber(Int16 i)
			: base()
		{
			this.Append(JavaScriptSerializer.Serialize(i));
		}

		public JavaScriptNumber(Int32 i)
			: base()
		{
			this.Append(JavaScriptSerializer.Serialize(i));
		}

		public JavaScriptNumber(Int64 i)
			: base()
		{
			this.Append(JavaScriptSerializer.Serialize(i));
		}

		public JavaScriptNumber(Double d)
			: base()
		{
			this.Append(JavaScriptSerializer.Serialize(d));
		}

		public JavaScriptNumber(Single s)
			: base()
		{
			this.Append(JavaScriptSerializer.Serialize(s));
		}

		/// <summary>
		/// Returns the string representation of the object.
		/// </summary>
		public string Value
		{
			get
			{
				return _value;
			}
		}

		#region Internal Methods

		internal void Append(string s)
		{
			_value += s;
		}

		internal void Append(char c)
		{
			_value += c;
		}

		internal int IndexOf(string s)
		{
			return _value.IndexOf(s);
		}

		#endregion

		#region Public Operators

		public override string ToString()
		{
			return this.Value;
		}

		public static implicit operator string(JavaScriptNumber o)
		{
			return o.Value;
		}

		public static implicit operator Int16(JavaScriptNumber o)
		{
			return Int16.Parse(o.Value);
		}

		public static implicit operator Int32(JavaScriptNumber o)
		{
			return Int32.Parse(o.Value);
		}

		public static implicit operator Int64(JavaScriptNumber o)
		{
			return Int64.Parse(o.Value);
		}

		public static implicit operator Double(JavaScriptNumber o)
		{
			return Double.Parse(o.Value);
		}

		public static implicit operator Single(JavaScriptNumber o)
		{
			return Single.Parse(o.Value);
		}

		public static JavaScriptNumber operator +(JavaScriptNumber a, string s)
		{
			a.Append(s);

			return a;
		}

		public static JavaScriptNumber operator +(JavaScriptNumber a, char c)
		{
			a.Append(c);

			return a;
		}

		#endregion
	}
}

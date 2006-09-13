/*
 * MS	06-04-03	return the correct .Value
 * MS	06-04-29	fixed ToString and Value properties
 * MS	06-05-31	added ctor for string and char
 * 
 */
using System;
using System.Collections;

namespace AjaxPro
{
	/// <summary>
	/// Represents a JavaScript ECMA string.
	/// </summary>
	public class JavaScriptString : IJavaScriptObject
	{
		private string _value = string.Empty;

		/// <summary>
		/// Initializes a new JavaScript string instance.
		/// </summary>
		public JavaScriptString()
			: base()
		{
		}

		public JavaScriptString(string s)
			: base()
		{
			this.Append(s);
		}

		public JavaScriptString(char c)
			: base()
		{
			this.Append(c);
		}

		/// <summary>
		/// Returns the string representation of the object.
		/// </summary>
		public string Value
		{
			get
			{
				return JavaScriptSerializer.Serialize(_value);
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

		#endregion

		#region Public Operators
		
		public override string ToString()
		{
			return _value;
		}

		public static implicit operator string(JavaScriptString o)
		{
			return o.ToString();
		}

		public static JavaScriptString operator +(JavaScriptString a, string s)
		{
			a.Append(s);

			return a;
		}

		public static JavaScriptString operator +(JavaScriptString a, char c)
		{
			a.Append(c);

			return a;
		}

		#endregion
	}
}

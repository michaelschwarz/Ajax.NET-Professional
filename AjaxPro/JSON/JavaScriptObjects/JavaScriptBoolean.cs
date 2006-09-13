/*
 * MS	06-04-03	return the correct .Value
 * MS	06-04-29	fixed ToString and Value properties
 * 
 * 
 * 
 */
using System;
using System.Collections;

namespace AjaxPro
{
	/// <summary>
	/// Represents a JavaScript ECMA boolean.
	/// </summary>
	public class JavaScriptBoolean : IJavaScriptObject
	{
		private bool _value = false;

		/// <summary>
		/// Initializes a new JavaScript boolean instance.
		/// </summary>
		public JavaScriptBoolean() : base()
		{

		}

		/// <summary>
		/// Initializes a new JavaScript boolean instance.
		/// </summary>
		/// <param name="value">The pre-defined value.</param>
		public JavaScriptBoolean(bool value) : base()
		{
			_value = value;
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

		#region Public Operators

		public override string ToString()
		{
			return bool.Parse(this.Value).ToString();
		}

		public static implicit operator bool(JavaScriptBoolean o)
		{
			return bool.Parse(o.Value);
		}

		public static implicit operator string(JavaScriptBoolean o)
		{
			return o.ToString();
		}

		#endregion
	}
}
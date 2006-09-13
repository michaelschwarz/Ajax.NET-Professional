/*
 * MS	06-06-19	initial version
 * 
 */
using System;
using System.Collections;

namespace AjaxPro
{
	/// <summary>
	/// Represents a JavaScript ECMA new Object source code.
	/// </summary>
	public class JavaScriptSource : IJavaScriptObject
	{
		private string _value = string.Empty;

		/// <summary>
		/// Initializes a new JavaScript string instance.
		/// </summary>
		public JavaScriptSource()
			: base()
		{
		}

		public JavaScriptSource(string s)
			: base()
		{
			this.Append(s);
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

		#endregion

		#region Public Operators
		
		public override string ToString()
		{
			return _value;
		}

		public static implicit operator string(JavaScriptSource o)
		{
			return o.ToString();
		}

		public static JavaScriptSource operator +(JavaScriptSource a, string s)
		{
			a.Append(s);

			return a;
		}

		#endregion
	}
}

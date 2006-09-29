/*
 * JavaScriptNumber.cs
 * 
 * Copyright © 2006 Michael Schwarz (http://www.ajaxpro.info).
 * All Rights Reserved.
 * 
 * Permission is hereby granted, free of charge, to any person 
 * obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without 
 * restriction, including without limitation the rights to use, 
 * copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be 
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR 
 * ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
/*
 * MS	06-04-26	added implicit operators for short,int,long
 * MS	06-05-31	added cstor for primitive types
 * BT	06-09-13	changed underlying string to StringBuilder object
 * 
 * 
 */
using System;
using System.Text;
using System.Collections;

namespace AjaxPro
{
	/// <summary>
	/// Represents a JavaScript ECMA number.
	/// </summary>
	public class JavaScriptNumber : IJavaScriptObject
	{
		private StringBuilder _value = new StringBuilder();

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
				return _value.ToString();
			}
		}

		#region Internal Methods

		internal void Append(string s)
		{
			_value.Append(s);
		}

		internal void Append(char c)
		{
			_value.Append(c);
		}

		internal int IndexOf(string s)
		{
			return _value.ToString().IndexOf(s);
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
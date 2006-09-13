/*
 * MS	06-04-03	return the correct .Value
 * MS	06-04-29	fixed ToString and Value properties
 * MS	06-05-31	added new ctor for initial array
 * MS	06-07-20	fixed Add method, removed second ambigous one
 * 
 */
using System;
using System.Collections;

namespace AjaxPro
{
	/// <summary>
	/// Represents a JavaScript ECMA array.
	/// </summary>
	public class JavaScriptArray : ArrayList, IJavaScriptObject
	{
		/// <summary>
		/// Initializes a new JavaScript array instance.
		/// </summary>
		public JavaScriptArray()
			: base()
		{
		}

		public JavaScriptArray(IJavaScriptObject[] items)
			: base()
		{
			for (int i = 0; i < items.Length; i++)
				this.Add(items[i]);
		}

		public new IJavaScriptObject this[int index]
		{
			get
			{
				return (IJavaScriptObject)base[index];
			}
		}

		public override int Add(object value)
		{
			if (value is IJavaScriptObject)
				return base.Add(value);

			throw new NotSupportedException();
		}

		/// <summary>
		/// Returns the string representation of the object.
		/// </summary>
		public string Value
		{
			get
			{
				return JavaScriptSerializer.Serialize(this.ToArray());
			}
		}

		public override string ToString()
		{
			return this.Value;
		}
	}
}

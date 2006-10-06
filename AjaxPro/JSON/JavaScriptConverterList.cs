/*
 * JavaScriptDeserializer.cs
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
 * MS	06-05-30	removed this code
 * MS	06-10-04	added code again for .NET 1.1 (Hashtable sort is different)
 * 
 * 
 */
#if(!NET20)
using System;
using System.Collections;

namespace AjaxPro
{
	/// <summary>
	/// Represents a IDictionary of JavaScriptConverters.
	/// </summary>
	internal class JavaScriptConverterList : IDictionary
	{
		private Hashtable keys;
		private ArrayList values;

		public JavaScriptConverterList()
		{
			keys = new Hashtable();
			values = new ArrayList();
		}

		public IJavaScriptConverter this[string key]
		{
			get
			{
				return (IJavaScriptConverter)this[key];
			}
		}

		public IJavaScriptConverter this[int index]
		{
			get
			{
				object o = values[index];
				return (IJavaScriptConverter)o;
			}
		}

		

		#region IDictionary Members

		public void Add(object key, object value)
		{
			if (value as IJavaScriptConverter == null)
				return;

			lock (this.SyncRoot)
			{
				int idx = values.Add(value);
				keys.Add(key, idx);
			}
		}

		public void Clear()
		{
			lock (this.SyncRoot)
			{
				values.Clear();
				keys.Clear();
			}
		}

		public bool ContainsKey(object key)
		{
			return Contains(key);
		}

		public bool Contains(object key)
		{
			return keys.Contains(key);
		}

		public IDictionaryEnumerator GetEnumerator()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public bool IsFixedSize
		{
			get { return false; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public ICollection Keys
		{
			get
			{
				return keys;
			}
		}

		public void Remove(object key)
		{
			lock (this.SyncRoot)
			{
				if (keys.Contains(key))
				{
					int idx = (int)keys[key];
					keys.Remove(key);
					values.RemoveAt(idx);
				}
			}
		}

		public ICollection Values
		{
			get
			{
				return values;
			}
		}

		public object this[object key]
		{
			get
			{
				return values[(int)keys[key]];
			}
			set
			{
				lock (this.SyncRoot)
				{
					values[(int)keys[key]] = value;
				}
			}
		}

		#endregion

#region ICollection Members

		public void CopyTo(Array array, int index)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public int Count
		{
			get
			{
				return keys.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return values.IsSynchronized;
			}
		}

		public object SyncRoot
		{
			get
			{
				return values.SyncRoot;
			}
		}

		#endregion

#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
#endif
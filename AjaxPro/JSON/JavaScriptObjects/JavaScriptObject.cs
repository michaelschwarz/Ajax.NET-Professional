/*
 * MS	06-04-03	return the correct .Value
 * MS	06-04-29	fixed ToString and Value properties
 * MS	06-05-31	using type safe generic list for .NET 2.0
 * 
 */
using System;
using System.Collections;
using System.Collections.Specialized;
#if(NET20)
using System.Collections.Generic;
#endif

namespace AjaxPro
{
	/// <summary>
	/// Represents a JavaScript ECMA object.
	/// </summary>
	public class JavaScriptObject : IJavaScriptObject
	{
#if(NET20)
		private Dictionary<string, IJavaScriptObject> list = new Dictionary<string, IJavaScriptObject>();
#else
		private HybridDictionary list = new HybridDictionary();
		private StringCollection keys = new StringCollection();
#endif

		/// <summary>
		/// Initializes a new JavaScript object instance.
		/// </summary>
		public JavaScriptObject() : base()
		{
		}

		/// <summary>
		/// Returns the string representation of the object.
		/// </summary>
		public string Value
		{
			get
			{
				return JavaScriptSerializer.Serialize(list);
			}
		}

		public override string ToString()
		{
			return this.Value;
		}

		#region IDictionary Members

		/// <summary>
		/// Returns the object defined for the name of the property.
		/// </summary>
		public IJavaScriptObject this[string key]
		{
			get
			{
#if(NET20)
				return list[key];
#else
				return (IJavaScriptObject)list[key];
#endif
			}
			set
			{
				if (!this.Contains(key))
					throw new ArgumentException("The specified key does not exists in this collection.", "key");

				list[key] = value;
			}
		}

		/// <summary>
		/// Verify if the property does exist in the object.
		/// </summary>
		/// <param name="key">The name of the property.</param>
		/// <returns>Returns true if the property is defined.</returns>
		public bool Contains(string key)
		{
#if(NET20)
			return list.ContainsKey(key);
#else
			return list.Contains(key);
#endif
		}

		/// <summary>
		/// Adds a new property to the object.
		/// </summary>
		/// <param name="key">The name of the property.</param>
		/// <param name="value">The value of the property.</param>
		public void Add(string key, IJavaScriptObject value)
		{
#if(NET20)
			list.Add(key, value);
#else
			keys.Add(key);
			list.Add(key, value);
#endif
		}

//        public void Add(string key, string value)
//        {
//#if(NET20)
//            list.Add(key, new JavaScriptString(value));
//#else
//            keys.Add(key);
//            list.Add(key, new JavaScriptString(value));
//#endif
//        }

		/// <summary>
		/// Returns all keys that are used internal for the name of properties.
		/// </summary>
		public string[] Keys
		{
			get
			{
#if(NET20)
				string[] _keys = new string[list.Count];
				list.Keys.CopyTo(_keys, 0);
#else
				string[] _keys = new string[keys.Count];
				keys.CopyTo(_keys, 0);
#endif

				return _keys;
			}
		}

		public bool IsFixedSize
		{
			get
			{
				return false;
			}
		}

		#endregion
	}
}


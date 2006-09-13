/*
 * MS	05-12-21	added Deserialize for Hashtables
 *					JavaScript object will now include the type for key and value
 * MS	06-04-25	removed unnecessarily used cast
 * MS	06-05-17	copy of old IDictionaryConverter
 * MS	06-05-23	using local variables instead of "new Type()" for get De-/SerializableTypes
 * 
 * 
 */
using System;
using System.Reflection;
using System.Text;
using System.Collections;
using System.Collections.Specialized;

namespace AjaxPro
{
	/// <summary>
	/// Provides methods to serialize and deserialize an object that implements IDictionary.
	/// </summary>
	public class HashtableConverter : IJavaScriptConverter
	{
		public HashtableConverter()
			: base()
		{
			m_serializableTypes = new Type[] { typeof(Hashtable) };
			m_deserializableTypes = new Type[] { typeof(Hashtable) };
		}

		public override object Deserialize(IJavaScriptObject o, Type t)
		{
			if (!(o is JavaScriptArray))
				throw new NotSupportedException();

			JavaScriptArray a = (JavaScriptArray)o;

			for (int i = 0; i < a.Count; i++)
				if (!(a[i] is JavaScriptArray))
					throw new NotSupportedException();

			IDictionary d = (IDictionary)Activator.CreateInstance(t);

			object key;
			object value;
			JavaScriptArray aa;

			for (int i = 0; i < a.Count; i++)
			{
				aa = (JavaScriptArray)a[i];
				key = JavaScriptDeserializer.Deserialize((IJavaScriptObject)aa[0], Type.GetType(((JavaScriptString)aa[2]).ToString()));
				value = JavaScriptDeserializer.Deserialize((IJavaScriptObject)aa[1], Type.GetType(((JavaScriptString)aa[3]).ToString()));

				d.Add(key, value);
			}

			return d;
		}

		public override string Serialize(object o)
		{
			IDictionary dic = o as IDictionary;

			if (dic == null)
				throw new NotSupportedException();

			StringBuilder sb = new StringBuilder();


			IDictionaryEnumerator enumerable = dic.GetEnumerator();

			enumerable.Reset();
			bool b = true;

			sb.Append("[");

			while (enumerable.MoveNext())
			{
				if (b) { b = false; }
				else { sb.Append(","); }

				sb.Append('[');
				sb.Append(JavaScriptSerializer.Serialize(enumerable.Key));
				sb.Append(',');
				sb.Append(JavaScriptSerializer.Serialize(enumerable.Value));
				sb.Append(',');
				sb.Append(JavaScriptSerializer.Serialize(enumerable.Key.GetType().FullName));
				sb.Append(',');
				sb.Append(JavaScriptSerializer.Serialize(enumerable.Value.GetType().FullName));
				sb.Append(']');
			}

			sb.Append("]");

			return sb.ToString();
		}
	}
}
/*
 * MS	06-05-23	using local variables instead of "new Type()" for get De-/SerializableTypes
 * MS	06.06.09	added IEnumerable type
 * MS	06-06-09	removed addNamespace use
 * MS	06-09-26	improved performance using StringBuilder
 * 
 */
using System;
using System.Reflection;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
#if(NET20)
using System.Collections.Generic;
#endif

namespace AjaxPro
{
	/// <summary>
	/// Provides methods to serialize and deserialize an object that implements IList.
	/// </summary>
	/// <remarks>
	/// The .Add methods argument type is used to get the type for the list.
	/// </remarks>
	public class IListConverter : IJavaScriptConverter
	{
		public IListConverter() : base()
		{
			m_AllowInheritance = true;

			m_serializableTypes = new Type[] {
#if(NET20)
				typeof(IList<>),
#endif
				typeof(IList), typeof(IEnumerable),
				typeof(string[]), typeof(int[]), typeof(bool[])
			};
			m_deserializableTypes = m_serializableTypes;
		}

		public override string Serialize(object o)
		{
			StringBuilder sb = new StringBuilder();
			Serialize(o, sb);
			return sb.ToString();
		}

		public override void Serialize(object o, StringBuilder sb)
		{
			IEnumerable enumerable = (IEnumerable)o;
			bool b = true;

			sb.Append("[");

			foreach (object obj in enumerable)
			{
				if (!b) sb.Append(",");

				sb.Append(JavaScriptSerializer.Serialize(obj));

				b = false;
			}

			sb.Append("]");
		}

		public override object Deserialize(IJavaScriptObject o, Type t)
		{
			JavaScriptArray list = o as JavaScriptArray;

			if (list == null)
				throw new NotSupportedException();

			if (t.IsArray)
			{
				Type type = Type.GetType(t.AssemblyQualifiedName.Replace("[]", ""));
				Array a = Array.CreateInstance(type, (list != null ? list.Count : 0));

				try
				{
					if (list != null)
					{
						for (int i = 0; i < list.Count; i++)
						{
							object v = JavaScriptDeserializer.Deserialize(list[i], type);
							a.SetValue(v, i);
						}
					}
				}
				catch (System.InvalidCastException iex)
				{
					throw new InvalidCastException("Array ('" + t.Name + "') could not be filled with value of type '" + type.Name + "'.", iex);
				}

				return a;
			}

			if(!typeof(IList).IsAssignableFrom(t) || !(o is JavaScriptArray))
				throw new NotSupportedException();

			IList l = (IList)Activator.CreateInstance(t);

			MethodInfo mi = t.GetMethod("Add");
			ParameterInfo pi = mi.GetParameters()[0];

			for(int i=0; i<list.Count; i++)
			{
				l.Add(JavaScriptDeserializer.Deserialize(list[i], pi.ParameterType));
			}

			return l;
		}

		public override bool TrySerializeValue(object o, Type t, StringBuilder sb)
		{
			if (t.IsArray) // || typeof(IEnumerable).IsAssignableFrom(t))
			{
				this.Serialize(o, sb);
				return true;
			}

			return base.TrySerializeValue(o, t, sb);
		}
	}
}

/*
 * MS	06-05-23	added de-/serialzableTypes variable instead of "new Type[]"
 * MS	06-05-30	added TrySerializeValue and TryDeserializeValue
 * MS	06-06-12	added StringDictionary argument for .Initialize
 * 
 */
using System;
using System.Text;
using System.Collections.Specialized;

namespace AjaxPro
{
	/// <summary>
	/// Represents an IJavaScriptConverter.
	/// </summary>
	public class IJavaScriptConverter
	{
		protected bool m_AllowInheritance = false;
		protected Type[] m_serializableTypes = new Type[0];
		protected Type[] m_deserializableTypes = new Type[0];

		/// <summary>
		/// Initializes the converter. This method will be called when the application is starting and 
		/// any converter is loaded.
		/// </summary>
		public virtual void Initialize(StringDictionary d)
		{
		}

		/// <summary>
		/// Render the JavaScript code for prototypes or any other JavaScript method needed from this converter
		/// on the client-side.
		/// </summary>
		/// <returns>Returns JavaScript code.</returns>
		public virtual string GetClientScript()
		{
			return "";
		}

		/// <summary>
		/// Converts a .NET object into a JSON string.
		/// </summary>
		/// <param name="o">The object to convert.</param>
		/// <returns>Returns a JSON string.</returns>
		public virtual string Serialize(object o)
		{
			throw new NotImplementedException("Converter for type '" + o.GetType().FullName + "'.");
		}

		/// <summary>
		/// Converts an IJavaScriptObject into an NET object.
		/// </summary>
		/// <param name="o">The IJavaScriptObject object to convert.</param>
		/// <param name="type">The type to convert the object to.</param>
		/// <returns>Returns a .NET object.</returns>
		public virtual object Deserialize(IJavaScriptObject o, Type t)
		{
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="o"></param>
		/// <param name="t"></param>
		/// <param name="json"></param>
		/// <returns></returns>
		public virtual bool TrySerializeValue(object o, Type t, out string json)
		{
			if (m_AllowInheritance)
			{
				for (int i = 0; i < m_serializableTypes.Length; i++)
				{
					if (m_serializableTypes[i].IsAssignableFrom(t))
					{
						json = this.Serialize(o);
						return true;
					}
				}
			}

			json = null;
			return false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="o"></param>
		/// <param name="t"></param>
		/// <param name="json"></param>
		/// <returns></returns>
		public virtual bool TryDeserializeValue(IJavaScriptObject jso, Type t, out object o)
		{
			if (m_AllowInheritance)
			{
				for (int i = 0; i < m_deserializableTypes.Length; i++)
				{
					if (m_deserializableTypes[i].IsAssignableFrom(t))
					{
						o = this.Deserialize(jso, t);
						return true;
					}
				}
			}

			o = null;
			return false;
		}

		/// <summary>
		/// Returns every type that can be used with this converter to serialize an object.
		/// </summary>
		public virtual Type[] SerializableTypes
		{
			get
			{
				return m_serializableTypes;
			}
		}

		/// <summary>
		/// Returns every type that can be used with this converter to deserialize an JSON string.
		/// </summary>
		public virtual Type[] DeserializableTypes
		{
			get
			{
				return m_deserializableTypes;
			}
		}

		public virtual string ConverterName
		{
			get
			{
				return this.GetType().Name;
			}
		}
	}
}
/*
* MS	05-12-21	added Deserialize for Hashtables
*					JavaScript object will now include the type for key and value
* MS	06-04-25	removed unnecessarily used cast
* MS	06-05-16	added Generic.IDictionary support
*					(initial version with new client-side script)
 * MS	06-05-23	using local variables instead of "new Type()" for get De-/SerializableTypes
 * MS	06-06-09	removed addNamespace use
 * MS	06-06-14	changed access to keys and values
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
	public class IDictionaryConverter : IJavaScriptConverter
	{
		private string clientType = "Ajax.Web.Dictionary";

		public IDictionaryConverter() : base()
		{
			m_AllowInheritance = true;
#if(NET20)
			m_serializableTypes = new Type[] { typeof(IDictionary), typeof(System.Collections.Generic.IDictionary<,>) };
			m_deserializableTypes = m_serializableTypes;
#else
			m_serializableTypes = new Type[] { typeof(IDictionary), typeof(NameValueCollection) };
			m_deserializableTypes = new Type[] { typeof(IDictionary), typeof(NameValueCollection) };
#endif
		}

		public override string GetClientScript()
		{
			return JavaScriptUtil.GetClientNamespaceRepresentation(clientType) + @"
" + clientType + @" = function(type,items) {
	this.__type = type;
	this.keys = [];
	this.values = [];

	if(items != null && !isNaN(items.length)) {
		for(var i=0; i<items.length; i++)
			this.add(items[i][0], items[i][1]);
	}
}
Object.extend(" + clientType + @".prototype, {
	add: function(k, v) {
		this.keys.push(k);
		this.values.push(v);
		return this.values.length -1;
	},
	containsKey: function(key) {
		for(var i=0; i<this.keys.length; i++)
			if(this.keys[i] == key) return true;
		return false;
	},
	getKeys: function() {
		return this.keys;
	},
	getValue: function(key) {
		for(var i=0; i<this.keys.length && i<this.values.length; i++)
			if(this.keys[i] == key) return this.values[i];
		return null;
	},
	setValue: function(k, v) {
		for(var i=0; i<this.keys.length && i<this.values.length; i++) {
			if(this.keys[i] == k) this.values[i] = v;
			return i;
		}
		return this.add(k, v);
	},
	toJSON: function() {
		return AjaxPro.toJSON({__type:this.__type,keys:this.keys,values:this.values});
	}
}, true);
";
		}

        public override object Deserialize(IJavaScriptObject o, Type t)
        {
			JavaScriptObject ht = o as JavaScriptObject;

            if (ht == null)
                throw new NotSupportedException();

            if (!ht.Contains("keys") || !ht.Contains("values"))
				throw new NotSupportedException();

            IDictionary d = (IDictionary)Activator.CreateInstance(t);

			ParameterInfo[] p = t.GetMethod("Add").GetParameters();

			Type kT = p[0].ParameterType;
			Type vT = p[1].ParameterType;

            object key;
            object value;

			JavaScriptArray keys = ht["keys"] as JavaScriptArray;
			JavaScriptArray values = ht["values"] as JavaScriptArray;

            for (int i = 0; i < keys.Count && i < values.Count; i++)
            {
                key = JavaScriptDeserializer.Deserialize(keys[i], kT);
				value = JavaScriptDeserializer.Deserialize(values[i], vT);

                d.Add(key, value);
            }

            return d;
        }

		public override string Serialize(object o)
		{
			IDictionary dic = o as IDictionary;

			if(dic == null)
				throw new NotSupportedException();

			StringBuilder sb = new StringBuilder();

			IDictionaryEnumerator enumerable = dic.GetEnumerator();

			enumerable.Reset();
			bool b = true;

			sb.Append("new ");
			sb.Append(clientType);
			sb.Append("(");

			bool readFirst = enumerable.MoveNext();		// read the first item
			Type t = o.GetType();

			sb.Append(JavaScriptSerializer.Serialize(t.FullName));
			sb.Append(",[");

			if (readFirst)
			{
				do
				{
					if (b) { b = false; }
					else { sb.Append(","); }

					sb.Append('[');
					sb.Append(JavaScriptSerializer.Serialize(enumerable.Key));
					sb.Append(',');
					sb.Append(JavaScriptSerializer.Serialize(enumerable.Value));
					sb.Append(']');
				}
				while (enumerable.MoveNext());
			}

			sb.Append("]");
			sb.Append(")");

			return sb.ToString();
		}

		public override bool TrySerializeValue(object o, Type t, out string json)
		{
#if(NET20)
			if (IsInterfaceImplemented(o, typeof(IDictionary)))
			{
				json = this.Serialize(o);
				return true;
			}
#endif

			return base.TrySerializeValue(o, t, out json);
		}
#if(NET20)
		internal static bool IsInterfaceImplemented(object obj, Type interfaceType)
		{
			if (obj == null)
				throw new ArgumentNullException("obj");

			return
				obj.GetType().FindInterfaces(
					new TypeFilter(
						delegate(Type type, object filter)
						{
							return (type.Name == ((Type)filter).Name) &&
									(type.Namespace == ((Type)filter).Namespace);
						}),
						interfaceType
						).Length == 1;
		}
#endif
	}
}

/*
 * JavaScriptDeserializer.cs
 * 
 * Copyright � 2023 Michael Schwarz (http://www.ajaxpro.info).
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
 * MS	06-04-11	fixed array deserializer
 *					fixed ArrayList deserializer, using Int64 if JavaScriptNumber, Boolean if JavaScriptBoolean
 * MS	06-04-12	fixed type char (BUG: will return "\"" instead of ""
 * MS	06-04-13	fixed type char when char is empty JSON string
 * MS	06-04-26	fixed missing enum deserializer
 *					added support for Nullable<>
 * MS	06-05-16	added Generic.IDictionary support
 * MS	06-05-22	added oldStyle/allowNumberBooleanAsString to be compatible
 * MS	06-05-23	added support for Enum that are not integers
 * MS	06-05-30	changed to new converter usage
 * MS	06-07-11	added generic method for DeserializeFromJson
 * MS	06-09-26	improved performance removing three-times cast
 * MS	21-10-27	added allowed customized types for JSON deserialization
 * MS	21-11-22	changed error message when type is not allowed
 * MS	21-11-29	added check for custom type deserialization
 * 
 */
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Security;

namespace AjaxPro
{
	/// <summary>
	/// Provides methods for deserializing .NET objects from a Java Script Object Notation (JSON) string to an .NET object.
	/// </summary>
	public sealed class JavaScriptDeserializer
	{
        /// <summary>
        /// Converts an JSON string into an NET object.
        /// </summary>
        /// <param name="json">The JSON string representation.</param>
        /// <param name="type">The type to convert the string to.</param>
        /// <returns>Returns a .NET object.</returns>
        /// <example>
        /// using System;
        /// using AjaxPro;
        /// namespace Demo
        /// {
        ///		public class WebForm1 : System.Web.UI.Page
        ///		{
        ///			private void Page_Load(object sender, System.EventArgs e)
        ///			{
        ///				string json = "[1,2,3,4,5,6]";
        ///				object o = JavaScriptDeserializer.Deserialize(json, typeof(int[]);
        ///				if(o != null)
        ///				{
        ///					foreach(int i in (int[])o)
        ///					{
        ///						Response.Write(i.ToString());
        ///					}
        ///				}
        ///			}
        ///		}
        /// }
        /// </example>
		public static object DeserializeFromJson(string json, Type type)
		{
			JSONParser p = new JSONParser();
			IJavaScriptObject o = p.GetJSONObject(json);

			return JavaScriptDeserializer.Deserialize(o, type);
		}

#if(NET20)
		
		public static T Deserialize<T>(IJavaScriptObject o)
		{
			return (T)Deserialize(o, typeof(T));
		}

        /// <summary>
        /// Converts an JSON string into an NET object.
        /// </summary>
        /// <param name="json">The JSON string representation.</param>
        /// <returns>Returns a type safe .NET object.</returns>
        /// <typeparam name="T">The type to create from JSON string.</typeparam>
		public static T DeserializeFromJson<T>(string json)
		{
			return (T)DeserializeFromJson(json, typeof(T));
		}
#endif

        /// <summary>
        /// Deserializes the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
		[Obsolete("The recommended alternative is AjaxPro.JavaScriptDeserializer.DeserializeFromJson(string, Type).")]
		public static object Deserialize(string json, Type type)
		{
			return DeserializeFromJson(json, type);
		}

        /// <summary>
        /// Converts an IJavaScriptObject into an NET object.
        /// </summary>
        /// <param name="o">The IJavaScriptObject object to convert.</param>
        /// <param name="type">The type to convert the object to.</param>
        /// <returns>Returns a .NET object.</returns>
		public static object Deserialize(IJavaScriptObject o, Type type)
		{
			if (o == null)
			{
				return null;
			}

			// If the IJavaScriptObject is an JavaScriptObject and we have a key
			// __type we will override the Type that is passed to this method. This
			// will allow us to use implemented classes where the method will use
			// only the interface.

			JavaScriptObject jso = o as JavaScriptObject;

			if (jso != null && jso.Contains("__type"))
			{
				string typeName = jso["__type"].ToString();

				// Validate the type name string before calling Type.GetType().
				// Type.GetType() can trigger static constructors and module initializers on
				// gadget types even before the allow-list check would run.
				ThrowExceptionIfNotCustomTypeNameDeserializationAllowed(typeName);

				Type t = Type.GetType(typeName);
				if (type == null || type.IsAssignableFrom(t))
				{
					type = t;
					ThrowExceptionIfNotCustomTypeDeserializationAllowed(type);
				}
			}

			IJavaScriptConverter c = null;
#if(NET20)
			if (Utility.Settings.DeserializableConverters.TryGetValue(type, out c))
			{
#else
			if(Utility.Settings.DeserializableConverters.ContainsKey(type))
			{
				c = (IJavaScriptConverter)Utility.Settings.DeserializableConverters[type];
#endif
				return c.Deserialize(o, type);
			}

			object v;
#if(NET20)
			foreach(IJavaScriptConverter c2 in Utility.Settings.DeserializableConverters.Values)
			{
				if(c2.TryDeserializeValue(o, type, out v))
					return v;
			}
#else
			IEnumerator m = Utility.Settings.DeserializableConverters.Values.GetEnumerator();
			while(m.MoveNext())
			{
				if (((IJavaScriptConverter)m.Current).TryDeserializeValue(o, type, out v))
					return v;
			}
#endif


#if(NET20)
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
			{
				//Type n = typeof(Nullable<>);
				//Type constructed = n.MakeGenericType(type.GetGenericArguments()[0]);

				Type tval = type.GetGenericArguments()[0];
				object va = Deserialize(o, tval);
				object val = Convert.ChangeType(va, tval);

				return Activator.CreateInstance(type, val);
			}
#endif

			if (typeof(IJavaScriptObject).IsAssignableFrom(type))
			{
				return o;
			}

			if (o is JavaScriptObject)
			{
				return DeserializeCustomObject(o as JavaScriptObject, type);
			}

			throw new NotImplementedException("The object of type '" + o.GetType().FullName + "' could not converted to type '" + type + "'.");
		}

		#region Internal Methods

		private const string TypeNotAllowedMessage = "At least one type passed is not allowed to be deserialized.";

		// Type names from a request are short. The limit keeps a deliberately deep
		// nesting of generic arguments from exhausting the stack.
		private const int MaxTypeNameNestingDepth = 8;

		internal static void ThrowExceptionIfNotCustomTypeDeserializationAllowed(Type type)
		{
			SecurityException ex = null;
			if (!IsCustomTypeDeserializationAllowed(type, out ex) && ex != null)
				throw ex;
		}

		internal static void ThrowExceptionIfNotCustomTypeNameDeserializationAllowed(string typeName)
		{
			SecurityException ex = null;
			if (!IsCustomTypeNameDeserializationAllowed(typeName, out ex) && ex != null)
				throw ex;
		}

		/// <summary>
		/// Validates a resolved type against the allow/deny configuration. Arrays are
		/// checked by their element type, generic types by their generic type definition
		/// (e.g. "System.Collections.Generic.List`1") and by every generic argument.
		/// </summary>
		internal static bool IsCustomTypeDeserializationAllowed(Type type, out SecurityException ex)
		{
			ex = null;

			// The type name passed in the request could not be resolved.
			if (type == null)
			{
				ex = new SecurityException(AjaxPro.Utility.Settings.DebugEnabled ? "A type passed could not be found." : TypeNotAllowedMessage);
				return false;
			}

			// Arrays, pointers and by-ref types are as safe as their element type.
			if (type.HasElementType)
				return IsCustomTypeDeserializationAllowed(type.GetElementType(), out ex);

			// allow all primitive and basic types
			if (type.IsPrimitive || type == typeof(string) || type == typeof(DateTime) || type == typeof(TimeSpan) || type == typeof(decimal))
				return true;

			string fullName = type.FullName;

			if (type.IsGenericType && !type.IsGenericTypeDefinition)
			{
				// Members typed with a generic argument are deserialized as that argument,
				// so every argument has to be allowed on its own.
				foreach (Type argument in type.GetGenericArguments())
					if (!IsCustomTypeDeserializationAllowed(argument, out ex))
						return false;

				fullName = type.GetGenericTypeDefinition().FullName;
			}

			// Open generic parameters have no full name and cannot be created.
			if (string.IsNullOrEmpty(fullName))
			{
				ex = new SecurityException(TypeNotAllowedMessage);
				return false;
			}

			return IsCustomTypeFullNameAllowed(fullName, out ex);
		}

		/// <summary>
		/// Validates a type name string against the allow/deny configuration without loading the type.
		/// Must be called before Type.GetType() to prevent gadget-chain attacks via static constructors.
		/// The name of the type itself and the names of all its generic arguments are validated.
		/// </summary>
		internal static bool IsCustomTypeNameDeserializationAllowed(string typeName, out SecurityException ex)
		{
			ex = null;

			if (string.IsNullOrEmpty(typeName))
				return true;

			System.Collections.Generic.List<string> fullNames = new System.Collections.Generic.List<string>();

			if (!TryGetFullNamesFromTypeName(typeName, fullNames, 0))
			{
				ex = new SecurityException(TypeNotAllowedMessage);
				return false;
			}

			foreach (string fullName in fullNames)
				if (!IsCustomTypeFullNameAllowed(fullName, out ex))
					return false;

			return true;
		}

		/// <summary>
		/// Collects the full name of the type and of all its generic arguments from a type
		/// name as accepted by Type.GetType(), without the assembly parts and array suffixes:
		/// "MyNamespace.MyClass, MyAssembly, Version=..." -> "MyNamespace.MyClass"
		/// "System.Collections.Generic.List`1[[MyNamespace.MyClass, MyAssembly]][], mscorlib"
		///     -> "System.Collections.Generic.List`1", "MyNamespace.MyClass"
		/// </summary>
		/// <returns>Returns false if the type name is malformed or uses syntax that is not supported.</returns>
		private static bool TryGetFullNamesFromTypeName(string typeName, System.Collections.Generic.List<string> fullNames, int depth)
		{
			if (depth > MaxTypeNameNestingDepth)
				return false;

			// The name ends at the first generic argument list, array suffix or assembly part.
			int pos = typeName.IndexOfAny(new char[] { '[', ']', ',' });
			string fullName = (pos < 0 ? typeName : typeName.Substring(0, pos)).Trim();

			// Reject if nothing remains (e.g. input was "  , SomeDangerousType" with a leading comma),
			// and reject escaped characters, pointers and by-ref types that are never sent by the client.
			if (fullName.Length == 0 || fullName.IndexOfAny(new char[] { '\\', '*', '&' }) >= 0)
				return false;

			fullNames.Add(fullName);

			// Any number of bracket groups can follow the name: one generic argument list
			// and array suffixes like [] or [,].
			while (pos >= 0 && pos < typeName.Length && typeName[pos] == '[')
			{
				int end = IndexOfClosingBracket(typeName, pos);
				if (end < 0)
					return false;

				string group = typeName.Substring(pos + 1, end - pos - 1);

				if (group.Trim(' ', ',', '*').Length > 0)
				{
					// A generic argument list: [[Type1, Assembly1],[Type2, Assembly2]] or [Type1,Type2]
					int start = 0;
					int nesting = 0;

					for (int i = 0; i <= group.Length; i++)
					{
						if (i < group.Length && group[i] == '[') nesting++;
						else if (i < group.Length && group[i] == ']') nesting--;
						else if (i == group.Length || (group[i] == ',' && nesting == 0))
						{
							string argument = group.Substring(start, i - start).Trim();
							start = i + 1;

							// An assembly qualified argument is enclosed in its own brackets.
							if (argument.Length > 1 && argument[0] == '[' && IndexOfClosingBracket(argument, 0) == argument.Length - 1)
								argument = argument.Substring(1, argument.Length - 2);

							if (!TryGetFullNamesFromTypeName(argument, fullNames, depth + 1))
								return false;
						}
					}
				}

				pos = end + 1;
				while (pos < typeName.Length && typeName[pos] == ' ') pos++;
			}

			// Only the assembly part is allowed to follow.
			return pos < 0 || pos >= typeName.Length || typeName[pos] == ',';
		}

		/// <summary>
		/// Returns the index of the bracket closing the one at the specified position, or -1.
		/// </summary>
		private static int IndexOfClosingBracket(string s, int open)
		{
			int nesting = 0;

			for (int i = open; i < s.Length; i++)
			{
				if (s[i] == '[') nesting++;
				else if (s[i] == ']' && --nesting == 0) return i;
			}

			return -1;
		}

		/// <summary>
		/// Validates the full name of a single type, without assembly part and generic
		/// arguments, against the allow/deny configuration.
		/// </summary>
		private static bool IsCustomTypeFullNameAllowed(string fullName, out SecurityException ex)
		{
			ex = null;

			// Allow the same primitive/system types that the type-object-based check passes through.
			if (fullName == "System.Boolean" || fullName == "System.Byte" || fullName == "System.SByte" ||
				fullName == "System.Char" || fullName == "System.Int16" || fullName == "System.UInt16" ||
				fullName == "System.Int32" || fullName == "System.UInt32" || fullName == "System.Int64" ||
				fullName == "System.UInt64" || fullName == "System.Single" || fullName == "System.Double" ||
				fullName == "System.IntPtr" || fullName == "System.UIntPtr" ||
				fullName == "System.String" || fullName == "System.DateTime" ||
				fullName == "System.TimeSpan" || fullName == "System.Decimal")
				return true;

			if (AjaxPro.Utility.Settings.IsCustomTypesDeserializationDisabled)
			{
				foreach (var s in AjaxPro.Utility.Settings.JsonDeserializationCustomTypesAllowed)
					if (IsMatchingTypeEntry(s, fullName))
						return true;
			}
			else
			{
				bool isCustomTypeDenied = false;

				foreach (var s in AjaxPro.Utility.Settings.JsonDeserializationCustomTypesDenied)
					if (IsMatchingTypeEntry(s, fullName))
					{
						isCustomTypeDenied = true;
						break;
					}

				if (!isCustomTypeDenied)
					return true;
			}

			ex = new SecurityException(AjaxPro.Utility.Settings.DebugEnabled ? "The type '" + fullName + "' is not allowed to be deserialized." : TypeNotAllowedMessage);
			return false;
		}

		/// <summary>
		/// Compares a full type name with an allow/deny entry, which is either a full
		/// type name or a prefix ending in * (e.g. "MyNamespace.*").
		/// </summary>
		private static bool IsMatchingTypeEntry(string entry, string fullName)
		{
			if (entry.EndsWith("*"))
				return fullName.StartsWith(entry.Substring(0, entry.Length - 1), StringComparison.InvariantCultureIgnoreCase);

			return string.Equals(entry, fullName, StringComparison.InvariantCultureIgnoreCase);
		}

        /// <summary>
        /// Deserializes the custom object.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
		internal static object DeserializeCustomObject(JavaScriptObject o, Type type)
		{
			object c = Activator.CreateInstance(type);

			MemberInfo[] members = type.GetMembers(BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance);
			foreach (MemberInfo memberInfo in members)
			{
				if (memberInfo.MemberType == MemberTypes.Field)
				{
					if (o.Contains(memberInfo.Name))
					{
						FieldInfo fieldInfo = (FieldInfo)memberInfo;

						fieldInfo.SetValue(c, JavaScriptDeserializer.Deserialize((IJavaScriptObject)o[fieldInfo.Name], fieldInfo.FieldType));
					}
				}
				else if (memberInfo.MemberType == MemberTypes.Property)
				{
					if (o.Contains(memberInfo.Name))
					{
						PropertyInfo propertyInfo = (PropertyInfo)memberInfo;

						if (propertyInfo.CanWrite)
							propertyInfo.SetValue(c, JavaScriptDeserializer.Deserialize((IJavaScriptObject)o[propertyInfo.Name], propertyInfo.PropertyType), new object[0]);
					}
				}
			}

			return c;
		}

		#endregion
	}
}



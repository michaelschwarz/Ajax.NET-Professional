/*
 * MS	06-04-03	fixed Decimal value, now using double instead
 * MS	06-04-04	added enum as integer instead of returning a class
 * MS	06-04-25	using float.IsNaN() instead of o == float.NaN
 * MS	06-04-29	added IJavaScript serializer
 * MS	06-05-09	fixed serialization of Exception if TargetSite is null
 * MS	06-05-17	fixed enum support for other types (insted of int we use Enum.GetUnderlyingType)
 * MS	06-05-30	changed to new converter usage
 * MS   06-07-10    fixed comma between fields and properties
 * 
 * 
 */
using System;
using System.Text;
using System.Reflection;
using System.Collections;

namespace AjaxPro
{
	/// <summary>
	/// Provides methods for serializing .NET objects to Java Script Object Notation (JSON).
	/// </summary>
	public sealed class JavaScriptSerializer
	{
		/// <summary>
		/// Converts a .NET object into a JSON string.
		/// </summary>
		/// <param name="o">The object to convert.</param>
		/// <returns>Returns a JSON string.</returns>
		/// <example>
		/// using System;
		/// using AjaxPro;
		/// 
		/// namespace Demo
		/// {
		///		public class WebForm1 : System.Web.UI.Page
		///		{
		///			private void Page_Load(object sender, System.EventArgs e)
		///			{
		///				DateTime serverTime = DateTime.Now;
		///				string json = JavaScriptSerializer.Serialize(serverTime);
		///				
		///				// json = "new Date(...)";
		///			}
		///		}
		/// }
		/// </example>
		public static string Serialize(object o)
		{
			if (o == null || o is System.DBNull)
			{
				return "null";
			}

			IJavaScriptConverter c = null;
			Type type = o.GetType();

#if(NET20)
			if (Utility.Settings.SerializableConverters.TryGetValue(type, out c))
			{
#else
			if(Utility.Settings.SerializableConverters.ContainsKey(type))
			{
				c = (IJavaScriptConverter)Utility.Settings.SerializableConverters[type];
#endif
				return c.Serialize(o);
			}

			string json;

			IEnumerator m = Utility.Settings.SerializableConverters.Values.GetEnumerator();
			while (m.MoveNext())
			{
				if (((IJavaScriptConverter)m.Current).TrySerializeValue(o, type, out json))
					return json;
			}

			try
			{
				json = SerializeCustomObject(o);
			}
			catch (StackOverflowException)
			{
				throw new Exception(Constant.AjaxID + " exception while trying to serialize type '" + type.Name + "'.");
			}

			return json;
		}

		/// <summary>
		/// Converts a string into a JSON string.
		/// </summary>
		/// <param name="s">The string to convert.</param>
		/// <returns>Returns a JSON string.</returns>
		[Obsolete("The recommended alternative is JavaScriptSerializer.Serialize(object).", false)]
		public static string SerializeString(string s)
		{
			return Serialize(s);
		}

		#region Internal Methods

		internal static string SerializeCustomObject(object o)
		{
			Type t = o.GetType();

			AjaxNonSerializableAttribute[] nsa = (AjaxNonSerializableAttribute[])t.GetCustomAttributes(typeof(AjaxNonSerializableAttribute), true);
			AjaxNoTypeUsageAttribute[] roa = (AjaxNoTypeUsageAttribute[])t.GetCustomAttributes(typeof(AjaxNoTypeUsageAttribute), true);

			StringBuilder sb = new StringBuilder();
			bool b = true;

			sb.Append('{');

			if (roa.Length == 0)
			{
				sb.Append("\"__type\":");
				sb.Append(Serialize(t.AssemblyQualifiedName));
				b = false;
			}

			FieldInfo[] fields = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo fi = fields[i];

				if ((nsa.Length > 0 && fi.GetCustomAttributes(typeof(AjaxPropertyAttribute), true).Length > 0) ||
					(nsa.Length == 0 && fi.GetCustomAttributes(typeof(AjaxNonSerializableAttribute), true).Length == 0))
				{
					if (b) { b = false; }
					else { sb.Append(','); }

					sb.Append(Serialize(fi.Name));
					sb.Append(':');
					sb.Append(Serialize(fi.GetValue(o)));
				}
			}

			PropertyInfo[] properties = t.GetProperties(BindingFlags.GetProperty | (BindingFlags.Public | BindingFlags.Instance));
			for (int i = 0; i < properties.Length; i++)
			{
				PropertyInfo prop = properties[i];

				MethodInfo mi = prop.GetGetMethod();
				if (mi.GetParameters().Length <= 0)
				{
					if ((nsa.Length > 0 && mi.GetCustomAttributes(typeof(AjaxPropertyAttribute), true).Length > 0) ||
						(nsa.Length == 0 && mi.GetCustomAttributes(typeof(AjaxNonSerializableAttribute), true).Length == 0))
					{
						if (b) { b = false; }
						else { sb.Append(","); }

						sb.Append(Serialize(prop.Name));
						sb.Append(':');
						sb.Append(Serialize(mi.Invoke(o, null)));
					}
				}
			}

			sb.Append('}');

			return sb.ToString();
		}

		#endregion
	}
}

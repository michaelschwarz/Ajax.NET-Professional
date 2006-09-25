/*
 * MS	06-04-03	fixed Decimal value, now using double instead
 * MS	06-04-04	added enum as integer instead of returning a class
 * MS	06-04-25	using float.IsNaN() instead of o == float.NaN
 * MS	06-04-29	added IJavaScript serializer
 * MS	06-05-09	fixed serialization of Exception if TargetSite is null
 * MS	06-05-17	fixed enum support for other types (insted of int we use Enum.GetUnderlyingType)
 * MS	06-05-30	changed to new converter usage
 * MS   06-07-10    fixed comma between fields and properties
 * MS	06-09-22	put some variable declarations outside of for statements
 * MS	06-09-26	improved performance using StringBuilder
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
		public static string Serialize(object o)
		{
			StringBuilder sb = new StringBuilder();
			Serialize(o, sb);
			return sb.ToString();
		}

		/// <summary>
		/// Converts a .NET object into a JSON string.
		/// </summary>
		/// <param name="o">The object to convert.</param>
		/// <param name="sb">A StringBuilder object.</param>
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
		public static void Serialize(object o, StringBuilder sb)
		{
			if (o == null || o is System.DBNull)
			{
				sb.Append("null");
				return;
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
				c.Serialize(o, sb);
				return;
			}

#if(NET20)
			foreach (IJavaScriptConverter c2 in Utility.Settings.SerializableConverters.Values)
			{
				if (c2.TrySerializeValue(o, type, sb))
					return;
			}
#else
			IEnumerator m = Utility.Settings.SerializableConverters.Values.GetEnumerator();
			while (m.MoveNext())
			{
				if (((IJavaScriptConverter)m.Current).TrySerializeValue(o, type, sb))
				{
					sb.ToString();
					return;
				}
			}
#endif

			try
			{
				SerializeCustomObject(o, sb);
				return;
			}
			catch (StackOverflowException)
			{
				throw new Exception(Constant.AjaxID + " stack overflow exception while trying to serialize type '" + type.Name + "'.");
			}

			throw new Exception(Constant.AjaxID + " exception while trying to serialize type '" + type.Name + "'.");
		}

		/// <summary>
		/// Converts a string into a JSON string.
		/// </summary>
		/// <param name="s">The string to convert.</param>
		/// <returns>Returns a JSON string.</returns>
		[Obsolete("The recommended alternative is JavaScriptUtil.QuoteString(string).", false)]
		public static string SerializeString(string s)
		{
			return JavaScriptUtil.QuoteString(s);
		}

		#region Internal Methods

		[Obsolete("The recommended alternative is JavaScriptSerializer.SerializeCustomObject(object, StringBuilder).", true)]
		internal static string SerializeCustomObject(object o)
		{
			StringBuilder sb = new StringBuilder();
			SerializeCustomObject(o, sb);
			return sb.ToString();
		}

		internal static void SerializeCustomObject(object o, StringBuilder sb)
		{
			Type t = o.GetType();

			//AjaxNonSerializableAttribute[] nsa = (AjaxNonSerializableAttribute[])t.GetCustomAttributes(typeof(AjaxNonSerializableAttribute), true);
			//AjaxNoTypeUsageAttribute[] roa = (AjaxNoTypeUsageAttribute[])t.GetCustomAttributes(typeof(AjaxNoTypeUsageAttribute), true);

			bool nsa = false;
			bool roa = false;

			foreach (object attr in t.GetCustomAttributes(true))
			{
				if (attr is AjaxNonSerializableAttribute) nsa = true;
				else if (attr is AjaxNoTypeUsageAttribute) roa = true;
			}

			bool b = true;

			sb.Append('{');

			if (!roa)
			{
				sb.Append("\"__type\":");
				JavaScriptUtil.QuoteString(t.AssemblyQualifiedName, sb);

				b = false;
			}

			foreach(FieldInfo fi in t.GetFields(BindingFlags.Public | BindingFlags.Instance))
			{
				if (
#if(NET20)
					(!nsa && !fi.IsDefined(typeof(AjaxNonSerializableAttribute), true)) ||
					(nsa && fi.IsDefined(typeof(AjaxPropertyAttribute), true))
#else
					(!nsa && fi.GetCustomAttributes(typeof(AjaxNonSerializableAttribute), true).Length == 0) ||
					(nsa && fi.GetCustomAttributes(typeof(AjaxPropertyAttribute), true).Length > 0)
#endif
)
				{
					if (!b) sb.Append(",");

					JavaScriptUtil.QuoteString(fi.Name, sb);
					sb.Append(':');
					Serialize(fi.GetValue(o), sb);

					b = false;
				}
			}

			foreach(PropertyInfo prop in t.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
			{
				MethodInfo mi = prop.GetGetMethod();
				if (mi.GetParameters().Length <= 0)
				{
					if (
#if(NET20)
						(!nsa && !mi.IsDefined(typeof(AjaxNonSerializableAttribute), true)) ||
						(nsa && mi.IsDefined(typeof(AjaxPropertyAttribute), true))
#else
						(!nsa && mi.GetCustomAttributes(typeof(AjaxPropertyAttribute), true).Length > 0) ||
						(nsa && mi.GetCustomAttributes(typeof(AjaxNonSerializableAttribute), true).Length == 0)
#endif
						)
					{
						if (!b) sb.Append(",");

						JavaScriptUtil.QuoteString(prop.Name, sb);
						sb.Append(':');
						Serialize(mi.Invoke(o, null), sb);

						b = false;
					}
				}
			}

			sb.Append('}');
		}

		#endregion
	}
}

/*
 * MS	06-05-23	using local variables instead of "new Type()" for get De-/SerializableTypes
 * MS	06-09-26	improved performance using StringBuilder
 * 
 * 
 */
using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AjaxPro
{
	/// <summary>
	/// Provides methods to serialize and deserialize an object that is inherited from HtmlControl.
	/// </summary>
	public class HtmlControlConverter : IJavaScriptConverter
	{
		public HtmlControlConverter() : base()
		{
			m_serializableTypes = new Type[]
				{
					typeof(HtmlControl),
					typeof(HtmlAnchor),
					typeof(HtmlButton),
					typeof(HtmlImage),
					typeof(HtmlInputButton),
					typeof(HtmlInputCheckBox),
					typeof(HtmlInputRadioButton),
					typeof(HtmlInputText),
					typeof(HtmlSelect),
					typeof(HtmlTableCell),
					typeof(HtmlTable),
					typeof(HtmlTableRow),
					typeof(HtmlTextArea)
				};
			m_deserializableTypes = new Type[]
				{
					typeof(HtmlControl),
					typeof(HtmlAnchor),
					typeof(HtmlButton),
					typeof(HtmlImage),
					typeof(HtmlInputButton),
					typeof(HtmlInputCheckBox),
					typeof(HtmlInputRadioButton),
					typeof(HtmlInputText),
					typeof(HtmlSelect),
					typeof(HtmlTableCell),
					typeof(HtmlTable),
					typeof(HtmlTableRow),
					typeof(HtmlTextArea)
				};
		}

		public override object Deserialize(IJavaScriptObject o, Type t)
		{
			if(!typeof(HtmlControl).IsAssignableFrom(t) || !(o is JavaScriptString))
				throw new NotSupportedException();

			return HtmlControlFromString(o.ToString(), t);
		}

		public override string Serialize(object o)
		{
			StringBuilder sb = new StringBuilder();
			Serialize(o, sb);
			return sb.ToString();
		}

		public override void Serialize(object o, StringBuilder sb)
		{
			if(!(o is Control))
				throw new NotSupportedException();

			sb.Append(HtmlControlToString((HtmlControl)o));
		}

		#region Internal Methods

		internal static string CorrectAttributes(string input)
		{
			string s = @"selected=""selected""";
			Regex r = new Regex(s, RegexOptions.Singleline | RegexOptions.IgnoreCase);
			input =  r.Replace(input, @"selected=""true""");

			s = @"multiple=""multiple""";
			r = new Regex(s, RegexOptions.Singleline | RegexOptions.IgnoreCase);
			input =  r.Replace(input, @"multiple=""true""");

			s = @"disabled=""disabled""";
			r = new Regex(s, RegexOptions.Singleline | RegexOptions.IgnoreCase);
			input =  r.Replace(input, @"disabled=""true""");

			return input;
		}

		internal static string HtmlControlToString(HtmlControl control)
		{
			StringWriter writer = new StringWriter(new StringBuilder());
			
			control.RenderControl(new HtmlTextWriter(writer));

			return JavaScriptSerializer.Serialize(writer.ToString());    
		}

		internal static HtmlControl HtmlControlFromString(string html, Type type)
		{
			if(!typeof(HtmlControl).IsAssignableFrom(type))
				throw new InvalidCastException("The target type is not a HtmlControlType");

			html = AddRunAtServer(html, (Activator.CreateInstance(type) as HtmlControl).TagName);

			if(type.IsAssignableFrom(typeof(HtmlSelect)))
				html = CorrectAttributes(html);

			Control o = HtmlControlConverterHelper.Parse(html);;
			
			if(o.GetType() == type)
				return (o as HtmlControl);
			else
			{
				foreach(Control con in o.Controls)
				{
					if(con.GetType() == type)
					{
						return (con as HtmlControl);
					}
				}
			}

			return null;
		}

		internal static string AddRunAtServer(string input, string tagName)
		{
			// <select[^>]*?(?<InsertPos>\s*)>
			string pattern = "<" + Regex.Escape(tagName) + @"[^>]*?(?<InsertPos>\s*)/?>";
			Regex regEx = new Regex(pattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
			Match match = regEx.Match(input);

			if (match.Success)
			{
				Group group = match.Groups["InsertPos"];
				return input.Insert(group.Index + group.Length, " runat=\"server\"");
			}
			else
				return input;
		}

		#endregion
	}

	internal class HtmlControlConverterHelper : TemplateControl
	{
		internal static Control Parse(string controlString)
		{
			HtmlControlConverterHelper control = new HtmlControlConverterHelper();
#if(NET20)
			control.AppRelativeVirtualPath = "~";
#endif

			return control.ParseControl(controlString);
		}
	}
}

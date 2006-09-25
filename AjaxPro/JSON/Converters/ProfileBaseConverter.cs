#if(NET20)
/*
 * MS	06-04-26	initial version
 * MS	06-05-23	using local variables instead of "new Type()" for get De-/SerializableTypes
 * MS	06-06-09	removed addNamespace use
 * MS	06-09-14	mark for NET20 only
 * MS	06-09-24	use QuoteString instead of Serialize
 * MS	06-09-26	improved performance using StringBuilder
 * 
 * 
 */
using System;
using System.Configuration;
using System.Web.Profile;
using System.Text;

namespace AjaxPro
{
	/// <summary>
	/// Provides methods to serialize and deserialize a ProfileBase object.
	/// </summary>
	public class ProfileBaseConverter : IJavaScriptConverter
	{
		private string clientType = "Ajax.Web.Profile";

		public ProfileBaseConverter()
			: base()
		{
			m_serializableTypes = new Type[] { typeof(ProfileBase) };
		}

		public override string GetClientScript()
		{
			return JavaScriptUtil.GetClientNamespaceRepresentation(clientType) + @"
" + clientType + @" = function() {
	this.toJSON = function() {
		throw """ + clientType + @" cannot be converted to JSON format."";
	}
	this.setProperty_callback = function(res) {
	}
	this.setProperty = function(name, object) {
		this[name] = object;
		AjaxPro.Services.Profile.SetProfile({name:o}, this.setProperty_callback.bind(this));
	}
}
";
		}

		public override string Serialize(object o)
		{
			StringBuilder sb = new StringBuilder();
			Serialize(o, sb);
			return sb.ToString();
		}

		public override void Serialize(object o, StringBuilder sb)
		{
			if (!(o is ProfileBase))
				throw new NotSupportedException();

			ProfileBase profile = (ProfileBase)o;

			bool b = true;

			sb.Append("Object.extend(new " + clientType + "(), {");

			foreach (SettingsProperty property in ProfileBase.Properties)
			{
				if (!b) sb.Append(",");

				JavaScriptUtil.QuoteString(property.Name, sb);
				sb.Append(":");
				JavaScriptSerializer.Serialize(profile[property.Name], sb);

				b = false;
			}

			sb.Append("})");
		}
	}
}
#endif
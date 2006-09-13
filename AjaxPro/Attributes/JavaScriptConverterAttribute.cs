using System;

namespace AjaxPro
{
	/// <summary>
	/// Represents an attribute to mark a class to be converted by a specified IJavaScriptConverter.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	[Obsolete("The recommended alternative is adding the converter to web.config/ajaxNet/ajaxSettings/jsonConverters.", true)]
	public class JavaScriptConverterAttribute : Attribute
	{
		private Type type = null;

		/// <summary>
		/// Marks a class to be converted by the specified JavaScript converter.
		/// </summary>
		/// <param name="type">The IJavaScriptConverter to use to serialize or deserialize.</param>
		public JavaScriptConverterAttribute(Type type)
		{
			if(!(typeof(IJavaScriptConverter).IsAssignableFrom(type)))
				throw new InvalidCastException();

			this.type = type;
		}

		#region Internal Methods

		internal IJavaScriptConverter Converter
		{
			get
			{
				object o = Activator.CreateInstance(type);
				return (IJavaScriptConverter)o;
			}
		}

		#endregion
	}
}

using System;

namespace AjaxPro
{
	/// <summary>
	/// Represents a JavaScript ECMA type.
	/// </summary>
	public interface IJavaScriptObject
	{
		/// <summary>
		/// Returns the string representation of the object.
		/// </summary>
		string Value{ get; }
	}
}

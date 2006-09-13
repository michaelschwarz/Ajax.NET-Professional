/*
 * MS	06-05-22	added UseDefault session state requirement
 * 
 * 
 * 
 */
using System;

namespace AjaxPro
{
	/// <summary>
	/// Represents the session state mode is required.
	/// </summary>
	public enum HttpSessionStateRequirement
	{
		/// <summary>
		/// Enables read/write access to the SessionState.
		/// </summary>
		ReadWrite,

		/// <summary>
		/// Enabales read access to the SessionState.
		/// </summary>
		Read,

		/// <summary>
		/// No SessionState available.
		/// </summary>
		None,

		/// <summary>
		/// Use default access to SessionState, see web.config configuration.
		/// </summary>
		UseDefault
	}
}

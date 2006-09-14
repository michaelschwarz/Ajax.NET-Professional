/*
 * MS	06-03-10	changed assembly version to correct version from assembly info
 * MS	06-04-04	fixed if external version is using different assembly name
 * 
 * 
 */
using System;

namespace AjaxPro
{
	public sealed class Constant
	{
		/// <summary>
		/// The AjaxID used to save objects using a unnique key in IDictionary objects.
		/// </summary>
		public const string AjaxID = "AjaxPro";

		/// <summary>
		/// The assembly name to get embedded resources
		/// </summary>
#if(NET20external)
		internal const string AssemblyName = "AjaxPro.2";
#else
		internal const string AssemblyName = "AjaxPro";
#endif

        /// <summary>
		/// The assembly version.
		/// </summary>
		public const string AssemblyVersion = "6.9.14.1";
	}
}

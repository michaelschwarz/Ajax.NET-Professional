/*
 * AjaxMethodAttribute.cs
 * 
 * Copyright © 2006 Michael Schwarz (http://www.ajaxpro.info).
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
 * MS	06-04-11	added new AjaxMethod argument for async httpHandler usage
 * MS	06-05-22	added obsolete beta attribute to async httpHandler usage
 * 
 * 
 */
using System;

namespace AjaxPro
{
	/// <summary>
	/// This Attribute must be used to create a AJAX wrapper.
	/// <code>
	/// public class Test
	/// {
	///		[AjaxPro.AjaxMethod]
	///		public string HelloWorld(string username)
	///		{
	///			return "Hello " + username;
	///		}
	///		
	///		[AjaxPro.AjaxMethod(AjaxPro.HttpSessionStateRequirement.ReadWrite)]
	///		public bool SessionValueIsSet(string key)
	///		{
	///			return System.Web.HttpContext.Current.Session[key] != null;
	///		}
	///	}
	/// </code>
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class AjaxMethodAttribute : Attribute
	{
		private bool useAsyncProcessing = false;
		private HttpSessionStateRequirement requireSessionState = HttpSessionStateRequirement.UseDefault;

		public AjaxMethodAttribute()
		{
		}

		/// <summary>
		/// Marks the method to be exported as an Ajax.NET Javascript function with the ability to access the SessionState.
		/// </summary>
		/// <param name="requireSessionState">The HttpSessionStateRequirement to use.</param>
		public AjaxMethodAttribute(HttpSessionStateRequirement requireSessionState)
		{
			this.requireSessionState = requireSessionState;
		}

		/// <summary>
		/// Marks the method to be exported as an Ajax.NET Javascript function with the ability to be processed as an async request on the server.
		/// </summary>
		/// <param name="useAsyncProcessing">The indicator if AsyncProcessing should be used.</param>
		[Obsolete("The use of this argument is currently in beta state, please report any problems to bug@schwarz-interactive.de.")]
		public AjaxMethodAttribute(bool useAsyncProcessing)
		{
			this.useAsyncProcessing = useAsyncProcessing;
		}

		/// <summary>
		/// Marks the method to be exported as an Ajax.NET Javascript function with the ability to be processed as an async request on the server and to access the SessionState.
		/// </summary>
		/// <param name="requireSessionState">The HttpSessionStateRequirement to use.</param>
		/// <param name="useAsyncProcessing">The indicator if AsyncProcessing should be used.</param>
		[Obsolete("The use of this argument is currently in beta state, please report any problems to bug@schwarz-interactive.de.")]
		public AjaxMethodAttribute(HttpSessionStateRequirement requireSessionState, bool useAsyncProcessing)
		{
			this.requireSessionState = requireSessionState;
			this.useAsyncProcessing = useAsyncProcessing;
		}

		#region Obsolete Constructors

		/// <summary>
		/// Marks the method to be exported as an Ajax.NET Javascript function with a different name.
		/// </summary>
		/// <param name="methodName">The name for the function to be used in Javascript.</param>
		[Obsolete("The recommended alternative is AjaxPro.AjaxNamespaceAttribute.", true)]
		public AjaxMethodAttribute(string methodName)
		{
		}

		[Obsolete("The recommended alternative is AjaxPro.AjaxServerCacheAttribute.", true)]
		public AjaxMethodAttribute(int cacheSeconds)
		{
		}

		[Obsolete("The recommended alternative is AjaxPro.AjaxServerCacheAttribute.", true)]
		public AjaxMethodAttribute(int cacheSeconds, HttpSessionStateRequirement requireSessionState)
		{
		}

		[Obsolete("The recommended alternative for methodName is AjaxPro.AjaxNamespaceAttribute.", true)]
		public AjaxMethodAttribute(string methodName, int cacheSeconds)
		{
		}

		[Obsolete("The recommended alternative for methodName is AjaxPro.AjaxNamespaceAttribute.", true)]
		public AjaxMethodAttribute(string methodName, int cacheSeconds, HttpSessionStateRequirement requireSessionState)
		{
		}

		[Obsolete("The recommended alternative for methodName is AjaxPro.AjaxNamespaceAttribute.", true)]
		public AjaxMethodAttribute(string methodName, HttpSessionStateRequirement requireSessionState)
		{
		}

		#endregion

		#region Internal Properties

		internal HttpSessionStateRequirement RequireSessionState
		{
			get{ return requireSessionState; }
		}

		internal bool UseAsyncProcessing
		{
			get { return useAsyncProcessing; }
		}

		#endregion
	}
}
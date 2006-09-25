/*
 * MS	06-09-26	put regex to private const
 * 
 * 
 * 
 */
using System;

namespace AjaxPro
{
	/// <summary>
	/// This attribute can be used to specified a different namespace for the client-side representation.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
	public class AjaxNamespaceAttribute : Attribute
	{
		private string _clientNS = null;
		private System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex("^[a-zA-Z_]{1}([a-zA-Z_]*([\\d]*)?)*((\\.)?[a-zA-Z_]+([\\d]*)?)*$", System.Text.RegularExpressions.RegexOptions.Compiled);

		/// <summary>
		/// This attribute can be used to specified a different namespace for the client-side representation.
		/// </summary>
		/// <param name="clientNS">The namespace to be used on the client-side JavaScript.</param>
		public AjaxNamespaceAttribute(string clientNS)
		{
            if(!r.IsMatch(clientNS) || clientNS.StartsWith(".") || clientNS.EndsWith("."))
                throw new NotSupportedException("The namespace '" + clientNS + "' is not supported.");

			_clientNS = clientNS;
		}

		#region Internal Properties

		internal string ClientNamespace
		{
			get
			{
                if (_clientNS != null && _clientNS.Trim().Length > 0)
                    return _clientNS.Replace("-", "_").Replace(" ", "_");

				return _clientNS;
			}
		}

		#endregion
	}
}

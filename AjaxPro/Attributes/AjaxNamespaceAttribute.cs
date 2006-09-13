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

		/// <summary>
		/// This attribute can be used to specified a different namespace for the client-side representation.
		/// </summary>
		/// <param name="clientNS">The namespace to be used on the client-side JavaScript.</param>
		public AjaxNamespaceAttribute(string clientNS)
		{
			string pattern = "^[a-zA-Z_]{1}([a-zA-Z_]*([\\d]*)?)*((\\.)?[a-zA-Z_]+([\\d]*)?)*$";

            if(!System.Text.RegularExpressions.Regex.IsMatch(clientNS, pattern) || clientNS.StartsWith(".") || clientNS.EndsWith("."))
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

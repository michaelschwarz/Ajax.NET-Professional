/*
 * MS	05-12-20	initial version
 * MS	06-04-16	changed methods to static
 * 
 * 
 * 
 * 
 */
using System;
using System.Web.Security;

namespace AjaxPro.Services
{
	[AjaxNamespace("AjaxPro.Services.Authentication")]
	public class AuthenticationService
	{
		[AjaxMethod]
		public static bool Login(string username, string password)
		{
#if(NET20)
			if(Membership.Provider.ValidateUser(username, password))
#else
			if(FormsAuthentication.Authenticate(username, password))
#endif
			{
				FormsAuthentication.SetAuthCookie(username, false);
				return true;
			}

			return false;
		}

		[AjaxMethod]
		public static void Logout()
		{
			FormsAuthentication.SignOut();
		}

		[AjaxMethod]
		public static bool ValidateUser(string username, string password)
		{
#if(NET20)
			return Membership.Provider.ValidateUser(username, password);
#else
			throw new NotImplementedException("ValidateUser is not yet implemented.");
#endif
		}
	}
}

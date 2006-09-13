#if(NET20)
/*
 * MS	05-12-20	initial version
 * MS	06-04-16	changed methods to static
 * MS	06-04-25	changed GetProfile, added ProfileBaseConverter
 * 
 * 
 * 
 */
using System;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Profile;

namespace AjaxPro.Services
{
	[AjaxNamespace("AjaxPro.Services.Profile")]
	public class ProfileService
	{
		[AjaxMethod]
		public static ProfileBase GetProfile()
		{
			return HttpContext.Current.Profile;
		}

        [AjaxMethod]
        public static object GetProfileProperty(string property)
        {
            ProfileBase profile = HttpContext.Current.Profile;
            if (profile == null)
            {
                return null;
            }
            return profile[property];
        }

        [AjaxMethod]
        public static bool SetProfile(JavaScriptObject o)
        {
            ProfileBase profile = HttpContext.Current.Profile;
            foreach (string key in o.Keys)
            {
				profile[key] = JavaScriptDeserializer.Deserialize((IJavaScriptObject)o[key], profile[key].GetType());
            }

			return true;
        }
	}
}
#endif
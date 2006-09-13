#if(NET20)
/*
 * MS	06-04-16	initial version
 * 
 * 
 * 
 * 
 * 
 */
using System;
using System.Text;

namespace AjaxPro.Services
{
	[AjaxNamespace("AjaxPro.Services.Cart")]
	public abstract class ICartService
	{
		[AjaxMethod]
		public abstract bool AddItem(string cartName, object item);

		[AjaxMethod]
		public abstract object[] GetItems(string cartName);
	}
}
#endif
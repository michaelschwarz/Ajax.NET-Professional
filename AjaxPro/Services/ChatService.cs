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
	[AjaxNamespace("AjaxPro.Services.Chat")]
	public abstract class IChatService
	{
		[AjaxMethod]
		public abstract bool SendMessage(string room, string message);

		[AjaxMethod]
		public abstract object[] RetrieveNew(string room, DateTime lastRetreived);

		[AjaxMethod]
		public abstract object[] RetrieveLast(string room, int count);
	}
}
#endif
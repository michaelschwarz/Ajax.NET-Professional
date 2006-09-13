using System;
using System.Configuration;
using System.Text;
#if(NET20)
using System.Configuration.Provider;
using System.Collections.Generic;
#endif

namespace AjaxPro
{
#if(NET20)
    public abstract class TokenProvider : ProviderBase
#else
	public abstract class TokenProvider
#endif
    {
        public TokenProvider()
        {
        }

        public abstract string GetToken();
        public abstract bool Parse(string token);
    }
}

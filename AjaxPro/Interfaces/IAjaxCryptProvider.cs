using System;

namespace AjaxPro
{
	public interface IAjaxCryptProvider
	{
		string Encrypt(string json);
		string Decrypt(string jsoncrypt);
		IAjaxKeyProvider KeyProvider{set;}
		string ClientScript{get;}
	}
}

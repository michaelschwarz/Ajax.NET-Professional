using System;
using System.Text;

namespace AjaxPro.Cryptography
{
	/// <summary>
	/// PC-Topp.NET encryptor for AuthenticationTicket, Drawings filename,...
	/// </summary>
	public class WebEncrypter
	{
		/// <summary>
		/// 
		/// </summary>
		public WebEncrypter()
		{
		}

		/// <summary>
		/// Encrypts a string.
		/// </summary>
		/// <param name="Text">The string should be encrypted.</param>
		/// <param name="Key">The 8-bit string for encryption.</param>
		/// <returns>The encrypted string.</returns>
		public string Encrypt(string Text, string Key)
		{
			if(Key.Length != 8)
				throw new Exception("Key must be a 8-bit string!");

			byte[] IV = null;
			byte[] cipherText = null;
			byte[] key = null;

			try
			{
				Encryptor enc = new Encryptor(EncryptionAlgorithm.Des);
				byte[] plainText = Encoding.ASCII.GetBytes(Text);

				key = Encoding.ASCII.GetBytes(Key);
				IV = Encoding.ASCII.GetBytes("init vec");		// "init vec is big."

				enc.IV = IV;

				cipherText = enc.Encrypt(plainText, key);
				IV = enc.IV;
				key = enc.Key;

			}
			catch(Exception ex)
			{
				throw new Exception("Exception while encrypting. " + ex.Message);
			}

			return Convert.ToBase64String(cipherText);
		}
	}
}





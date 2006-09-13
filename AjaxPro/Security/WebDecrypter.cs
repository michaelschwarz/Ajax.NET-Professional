using System;
using System.Text;

namespace AjaxPro.Cryptography
{
	/// <summary>
	/// PC-Topp.NET decryptor for AuthenticationTicket, Drawings filename,...
	/// </summary>
	public class WebDecrypter
	{
		/// <summary>
		/// 
		/// </summary>
		public WebDecrypter()
		{
		}

		/// <summary>
		/// Decrypts a string.
		/// </summary>
		/// <param name="Text">The encrypted string to be decrypted.</param>
		/// <param name="Key">The 8-bit string for decryption.</param>
		/// <returns>The plain text.</returns>
		public string Decrypt(string Text, string Key)
		{
			if(Key.Length != 8)
				throw new Exception("Key must be a 8-bit string!");

			byte[] IV = null;
			byte[] cipherText = null;
			byte[] key = null;
			byte[] plainText = null;

			try
			{
				Decryptor dec = new Decryptor(EncryptionAlgorithm.Des);

				IV = Encoding.ASCII.GetBytes("init vec");		// "init vec is big."

				dec.IV = IV;

				key = Encoding.ASCII.GetBytes(Key);
				cipherText = Convert.FromBase64String(Text);

				plainText = dec.Decrypt(cipherText, key);
			}
			catch(Exception ex)
			{
				throw new Exception("Exception while decrypting. " + ex.Message);
			}

			return Encoding.ASCII.GetString(plainText);
		}
	}
}

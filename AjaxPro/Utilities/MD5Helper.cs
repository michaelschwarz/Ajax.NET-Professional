using System;
using System.Text;
using System.Security.Cryptography;

namespace AjaxPro
{
	/// <summary>
	/// Provides methods to get a MD5 hash from a string or byte array.
	/// </summary>
	internal class MD5Helper
	{
		internal static string GetHash(string data)
		{
			byte[] b = System.Text.Encoding.Default.GetBytes(data);

			return GetHash(b);
		}

		internal static string GetHash(byte[] data)
		{
			string sMD5HashHexa = "";
			
			string[] tabStringHexa = new string[16];

			// This is one implementation of the abstract class MD5.
			MD5 md5 = new MD5CryptoServiceProvider();

			byte[] result = md5.ComputeHash(data);

			for (int i = 0; i < result.Length; i++) 
			{
				tabStringHexa[i] = (result[i]).ToString( "x" );
				sMD5HashHexa += tabStringHexa[i];
			}

			return sMD5HashHexa;
		}
	}
}

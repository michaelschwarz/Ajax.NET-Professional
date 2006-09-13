using System;
using System.IO;
using System.Security.Cryptography;

namespace AjaxPro.Cryptography
{
	/// <summary>
	/// General class for decryption.
	/// </summary>
	public class Decryptor
	{
		private DecryptTransformer transformer;
		private byte[] initVec;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="algId">The algorithm to decrypt data.</param>
		public Decryptor(EncryptionAlgorithm algId)
		{
			transformer = new DecryptTransformer(algId);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="bytesData"></param>
		/// <param name="bytesKey"></param>
		/// <returns></returns>
		public byte[] Decrypt(byte[] bytesData, byte[] bytesKey)
		{
			MemoryStream memStreamDecryptedData = new MemoryStream();

			transformer.IV = initVec;

			ICryptoTransform transform = transformer.GetCryptoServiceProvider(bytesKey);
			CryptoStream decStream = new CryptoStream(memStreamDecryptedData, transform, CryptoStreamMode.Write);

			try
			{
				decStream.Write(bytesData, 0, bytesData.Length);
			}
			catch(Exception ex)
			{
				throw new Exception("Error while writing encrypted data to the stream: \n" + ex.Message);
			}

			decStream.FlushFinalBlock();
			decStream.Close();

			return memStreamDecryptedData.ToArray();
		}

		/// <summary>
		/// 
		/// </summary>
		public byte[] IV
		{
			set
			{
				initVec = value;
			}
		}
	}
}

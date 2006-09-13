using System;
using System.IO;
using System.Security.Cryptography;

namespace AjaxPro.Cryptography
{
	/// <summary>
	/// General class for encryption.
	/// </summary>
	public class Encryptor
	{
		private EncryptTransformer transformer;
		private byte[] initVec;
		private byte[] encKey;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="algId">The algorithm to encrypt data.</param>
		public Encryptor(EncryptionAlgorithm algId)
		{
			transformer = new EncryptTransformer(algId);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="bytesData"></param>
		/// <param name="bytesKey"></param>
		/// <returns></returns>
		public byte[] Encrypt(byte[] bytesData, byte[] bytesKey)
		{
			MemoryStream memStreamEncryptedData = new MemoryStream();

			transformer.IV = initVec;

			ICryptoTransform transform = transformer.GetCryptoServiceProvider(bytesKey);
			CryptoStream encStream = new CryptoStream(memStreamEncryptedData, transform, CryptoStreamMode.Write);

			try
			{
				encStream.Write(bytesData, 0, bytesData.Length);
			}
			catch(Exception ex)
			{
				throw new Exception("Error while writing encrypted data to the stream: \n" + ex.Message);
			}

			encKey = transformer.Key;
			initVec = transformer.IV;

			encStream.FlushFinalBlock();
			encStream.Close();

			return memStreamEncryptedData.ToArray();
		}

		/// <summary>
		/// 
		/// </summary>
		public byte[] IV
		{
			get
			{
				return initVec;
			}
			set
			{
				initVec = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public byte[] Key
		{
			get
			{
				return encKey;
			}
		}

	}
}

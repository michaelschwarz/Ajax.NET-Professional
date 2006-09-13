/*
 * MS	06-04-25	enums should have a zero value
 * 
 * 
 */
using System;
using System.Security.Cryptography;

namespace AjaxPro.Cryptography
{
	/// <summary>
	/// 
	/// </summary>
	public enum EncryptionAlgorithm
	{
		/// <summary>
		/// 
		/// </summary>
		Des = 0,
		
		/// <summary>
		/// 
		/// </summary>
		Rc2,
		
		/// <summary>
		/// 
		/// </summary>
		Rijndael,
		
		/// <summary>
		/// 
		/// </summary>
		TripleDes
	};

	/// <summary>
	/// 
	/// </summary>
	internal class EncryptTransformer
	{
		private EncryptionAlgorithm algorithmID;
		private byte[] initVec;
		private byte[] encKey;

		public EncryptTransformer(EncryptionAlgorithm algId)
		{
			algorithmID = algId;
		}

		internal ICryptoTransform GetCryptoServiceProvider(byte[] bytesKey)
		{
			switch(algorithmID)
			{
				case EncryptionAlgorithm.Des:
					DES des = new DESCryptoServiceProvider();
					des.Mode = CipherMode.CBC;

					if(null == bytesKey)
					{
						encKey = des.Key;
					}
					else
					{
						des.Key = bytesKey;
						encKey = des.Key;
					}

					if(null == initVec)
					{
						initVec = des.IV;
					}
					else
					{
						des.IV = initVec;
					}
					return des.CreateEncryptor();

				case EncryptionAlgorithm.TripleDes:
					TripleDES des3 = new TripleDESCryptoServiceProvider();
					des3.Mode = CipherMode.CBC;

					if(null == bytesKey)
					{
						encKey = des3.Key;
					}
					else
					{
						des3.Key = bytesKey;
						encKey = des3.Key;
					}

					if(null == initVec)
					{
						initVec = des3.IV;
					}
					else
					{
						des3.IV = initVec;
					}
					return des3.CreateEncryptor();

				case EncryptionAlgorithm.Rc2:
					RC2 rc2 = new RC2CryptoServiceProvider();
					rc2.Mode = CipherMode.CBC;

					if(null == bytesKey)
					{
						encKey = rc2.Key;
					}
					else
					{
						rc2.Key = bytesKey;
						encKey = rc2.Key;
					}

					if(null == initVec)
					{
						initVec = rc2.IV;
					}
					else
					{
						rc2.IV = initVec;
					}
					return rc2.CreateEncryptor();

				case EncryptionAlgorithm.Rijndael:
					Rijndael rijndael = new RijndaelManaged();
					rijndael.Mode = CipherMode.CBC;

					if(null == bytesKey)
					{
						encKey = rijndael.Key;
					}
					else
					{
						rijndael.Key = bytesKey;
						encKey = rijndael.Key;
					}

					if(null == initVec)
					{
						initVec = rijndael.IV;
					}
					else
					{
						rijndael.IV = initVec;
					}
					return rijndael.CreateEncryptor();

				default:
					throw new CryptographicException("Algorithm ID '" + algorithmID + "' not supported!");
			}
		}

		internal byte[] IV
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

		internal byte[] Key
		{
			get
			{
				return encKey;
			}
		}

	}
}

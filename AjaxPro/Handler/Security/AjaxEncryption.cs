using System;

namespace AjaxPro
{
	internal class AjaxEncryption
	{
		private string m_CryptType = null;
		private string m_KeyType = null;
		private IAjaxCryptProvider m_CryptProvider = null;
		private IAjaxKeyProvider m_KeyProvider = null;

		internal AjaxEncryption(string cryptType, string keyType)
		{
			m_CryptType = cryptType;
			m_KeyType = keyType;
		}

		internal bool Init()
		{
			try
			{
				m_CryptProvider = (IAjaxCryptProvider)Activator.CreateInstance(Type.GetType(m_CryptType), new object[0]);
				m_KeyProvider = (IAjaxKeyProvider)Activator.CreateInstance(Type.GetType(m_KeyType), new object[0]);

				m_CryptProvider.KeyProvider = m_KeyProvider;

				return true;
			}
			catch(Exception)
			{
			}

			return false;
		}

		#region Public Properties

		public IAjaxCryptProvider CryptProvider
		{
			get{ return m_CryptProvider; }
		}

		public IAjaxKeyProvider KeyProvider
		{
			get{ return m_KeyProvider; }
		}

		#endregion
	}
}

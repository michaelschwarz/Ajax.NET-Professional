/*
 * AjaxEncryption.cs
 * 
 * Copyright © 2006 Michael Schwarz (http://www.ajaxpro.info).
 * All Rights Reserved.
 * 
 * Permission is hereby granted, free of charge, to any person 
 * obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without 
 * restriction, including without limitation the rights to use, 
 * copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be 
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR 
 * ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
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

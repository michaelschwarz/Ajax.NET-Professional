/*
 * MD5Helper.cs
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

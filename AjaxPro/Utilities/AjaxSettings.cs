/*
 * AjaxSettings.cs
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
/*
 * MS	06-04-04	added DebugEnabled web.config property <debug enabled="true"/>
 * MS	06-04-05	added OldStyle string collection (see web.config)
 * MS	06-04-12	added UseAssemblyQualifiedName (see web.config)
 * MS	06-05-30	changed to new converter dictionary
 * MS	06-06-07	added OnlyAllowTypesInList property (see web.config)
 * MS	06-10-06	using new JavaScriptConverterList for .NET 1.1
 * 
 */
using System;
using System.Collections;
#if(NET20)
using System.Collections.Generic;
#endif

namespace AjaxPro
{
#if(JSONLIB)
    internal class AjaxSettings
    {
		private System.Collections.Specialized.StringCollection m_OldStyle = new System.Collections.Specialized.StringCollection();

#if(NET20)
		internal Dictionary<Type, IJavaScriptConverter> SerializableConverters;
		internal Dictionary<Type, IJavaScriptConverter> DeserializableConverters;
#else
		internal Hashtable SerializableConverters;
		internal Hashtable DeserializableConverters;
#endif
    	/// <summary>
		/// Initializes a new instance of the <see cref="AjaxSettings"/> class.
		/// </summary>
		internal AjaxSettings()
		{
#if(NET20)
			SerializableConverters = new Dictionary<Type, IJavaScriptConverter>();
			DeserializableConverters = new Dictionary<Type, IJavaScriptConverter>();
#else
			SerializableConverters = new Hashtable();
			DeserializableConverters = new Hashtable();
#endif
		}

		/// <summary>
		/// Gets or sets several settings that will be used for old styled web applications.
		/// </summary>
		internal System.Collections.Specialized.StringCollection OldStyle
		{
			get { return m_OldStyle; }
			set { m_OldStyle = value; }
		}
    }
#else
	internal class AjaxSettings
	{
		private System.Collections.Hashtable m_UrlNamespaceMappings = new System.Collections.Hashtable();

		private bool m_DebugEnabled = false;
		private bool m_UseAssemblyQualifiedName = false;

		private System.Collections.Specialized.StringCollection m_OldStyle = new System.Collections.Specialized.StringCollection();

		private AjaxEncryption m_AjaxEncryption = null;
		private bool m_TokenEnabled = false;
		private string m_TokenSitePassword = "ajaxpro";

#if(NET20)
		internal Dictionary<Type, IJavaScriptConverter> SerializableConverters;
		internal Dictionary<Type, IJavaScriptConverter> DeserializableConverters;
#else
		internal JavaScriptConverterList SerializableConverters;
		internal JavaScriptConverterList DeserializableConverters;
#endif
		private bool m_OnlyAllowTypesInList = false;

		private string m_CoreScript = null;
		private System.Collections.Specialized.StringDictionary m_ScriptReplacements = new System.Collections.Specialized.StringDictionary();

		/// <summary>
		/// Initializes a new instance of the <see cref="AjaxSettings"/> class.
		/// </summary>
		internal AjaxSettings()
		{
#if(NET20)
			SerializableConverters = new Dictionary<Type, IJavaScriptConverter>();
			DeserializableConverters = new Dictionary<Type, IJavaScriptConverter>();
#else
			SerializableConverters = new JavaScriptConverterList();
			DeserializableConverters = new JavaScriptConverterList();
#endif
		}

		#region Public Properties

		/// <summary>
		/// Gets or sets the URL namespace mappings.
		/// </summary>
		/// <value>The URL namespace mappings.</value>
		internal System.Collections.Hashtable UrlNamespaceMappings
		{
			get{ return m_UrlNamespaceMappings; }
			set{ m_UrlNamespaceMappings = value; }
		}

		internal bool OnlyAllowTypesInList
		{
			get { return m_OnlyAllowTypesInList; }
			set { m_OnlyAllowTypesInList = value; }
		}

		/// <summary>
		/// Gets or sets if debug information should be enabled.
		/// </summary>
		internal bool DebugEnabled
		{
			get { return m_DebugEnabled; }
			set { m_DebugEnabled = value; }
		}

		/// <summary>
		/// Gets or sets the use of the AssemblyQualifiedName.
		/// </summary>
		internal bool UseAssemblyQualifiedName
		{
			get { return m_UseAssemblyQualifiedName; }
			set { m_UseAssemblyQualifiedName = value; }
		}

		/// <summary>
		/// Gets or sets several settings that will be used for old styled web applications.
		/// </summary>
		internal System.Collections.Specialized.StringCollection OldStyle
		{
			get { return m_OldStyle; }
			set { m_OldStyle = value; }
		}

		/// <summary>
		/// Gets or sets the encryption.
		/// </summary>
		/// <value>The encryption.</value>
		internal AjaxEncryption Encryption
		{
			get { return m_AjaxEncryption; }
			set { m_AjaxEncryption = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether token use is enabled.
		/// </summary>
		/// <value><c>true</c> if [token enabled]; otherwise, <c>false</c>.</value>
		internal bool TokenEnabled
		{
			get{ return m_TokenEnabled; }
			set{ m_TokenEnabled = value; }
		}

		/// <summary>
		/// Gets or sets the token site password.
		/// </summary>
		/// <value>The token site password.</value>
		internal string TokenSitePassword
		{
			get{ return m_TokenSitePassword; }
			set{ m_TokenSitePassword = value; }
		}

		[Obsolete("The recommended alternative is to configure a scriptReplacement/file in web.config.", true)]
		internal string CoreScript
		{
			get{ return m_CoreScript; }
			set{ m_CoreScript = value; }
		}

		/// <summary>
		/// Gets or sets the script replacements.
		/// </summary>
		/// <value>The script replacements.</value>
		internal System.Collections.Specialized.StringDictionary ScriptReplacements
		{
			get{ return m_ScriptReplacements; }
			set{ m_ScriptReplacements = value; }
		}

		#endregion
	}
#endif
}

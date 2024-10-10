/*
 * AjaxSettings.cs
 * 
 * Copyright © 2023 Michael Schwarz (http://www.ajaxpro.info).
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
 * MS	07-04-24	added IncludeTypeProperty
 *					added UseSimpleObjectNaming
 *					using new AjaxSecurityProvider
 *					fixed Ajax token
 * MS	21-10-27	added allowed customized types for JSON deserialization
 * MS	21-10-30	added contentSecurityPolicy to specify a nonce for all scripts
 * MS	21-11-22	changed to set the default behavior to not allow custom types
 * MS   23-05-25    added a configuration to not throw an exception when a property is not supported to read from
 * MS   24-10-10    added configuration ExceptionDetailsEnabled to hide exception detials
 * 
 * 
 */
using System;
#if(NET20)
using System.Collections.Generic;
#endif

namespace AjaxPro
{
#if (JSONLIB)
    internal class AjaxSettings
    {
		private System.Collections.Specialized.StringCollection m_OldStyle = new System.Collections.Specialized.StringCollection();
		private bool m_IncludeTypeProperty = false;
		
#if (NET20)
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
#if (NET20)
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

		internal bool IncludeTypeProperty
		{
			get { return m_IncludeTypeProperty; }
			set { m_IncludeTypeProperty = value; }
		}
    }
#else
    internal class AjaxSettings
    {
        private System.Collections.Hashtable m_UrlNamespaceMappings = new System.Collections.Hashtable();

        private bool m_IsDebugEnabled = false;
        private bool m_IsUseAssemblyQualifiedName = false;
        private bool m_IsIncludeTypeProperty = false;
        private bool m_IsUseSimpleObjectNaming = false;
        private bool m_IsOnlyAllowTypesInList = false;
        private bool m_IsIgnoreNotSupportedProperties = false;
        private bool m_ExceptionDetailsEnabled = false;

        private System.Collections.Specialized.StringCollection m_OldStyle = new System.Collections.Specialized.StringCollection();

        private AjaxSecurity m_AjaxSecurity = null;
        private string m_TokenSitePassword = "ajaxpro";

#if (NET20)
        internal Dictionary<Type, IJavaScriptConverter> SerializableConverters;
        internal Dictionary<Type, IJavaScriptConverter> DeserializableConverters;
#else
		internal JavaScriptConverterList SerializableConverters;
		internal JavaScriptConverterList DeserializableConverters;
#endif
        private string m_TypeJavaScriptProvider = null;

        private string m_CoreScript = null;
        private System.Collections.Specialized.StringDictionary m_ScriptReplacements = new System.Collections.Specialized.StringDictionary();

        /// <summary>
        /// Initializes a new instance of the <see cref="AjaxSettings"/> class.
        /// </summary>
		internal AjaxSettings()
        {
#if (NET20)
            SerializableConverters = new Dictionary<Type, IJavaScriptConverter>();
            DeserializableConverters = new Dictionary<Type, IJavaScriptConverter>();
#else
			SerializableConverters = new JavaScriptConverterList();
			DeserializableConverters = new JavaScriptConverterList();
#endif

            JsonDeserializationCustomTypesAllowed = new List<string>();
            JsonDeserializationCustomTypesDenied = new List<string>();

            // disable all custom types by default, either add allow list (or not recommended change default to 'allow')
            IsCustomTypesDeserializationDisabled = true;
        }

        #region Public Properties

        internal string TypeJavaScriptProvider
        {
            get { return m_TypeJavaScriptProvider; }
            set { m_TypeJavaScriptProvider = value; }
        }

        /// <summary>
        /// Gets or sets the URL namespace mappings.
        /// </summary>
        /// <value>The URL namespace mappings.</value>
		internal System.Collections.Hashtable UrlNamespaceMappings
        {
            get { return m_UrlNamespaceMappings; }
            set { m_UrlNamespaceMappings = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [only allow types in list].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [only allow types in list]; otherwise, <c>false</c>.
        /// </value>
		internal bool OnlyAllowTypesInList
        {
            get { return m_IsOnlyAllowTypesInList; }
            set { m_IsOnlyAllowTypesInList = value; }
        }

        /// <summary>
        /// Gets or sets if debug information should be enabled.
        /// </summary>
        /// <value><c>true</c> if [debug enabled]; otherwise, <c>false</c>.</value>
		internal bool DebugEnabled
        {
            get { return m_IsDebugEnabled; }
            set { m_IsDebugEnabled = value; }
        }

        /// <summary>
        /// Gets or sets the use of the AssemblyQualifiedName.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [use assembly qualified name]; otherwise, <c>false</c>.
        /// </value>
		internal bool UseAssemblyQualifiedName
        {
            get { return m_IsUseAssemblyQualifiedName; }
            set { m_IsUseAssemblyQualifiedName = value; }
        }

        internal bool IncludeTypeProperty
        {
            get { return m_IsIncludeTypeProperty; }
            set { m_IsIncludeTypeProperty = value; }
        }

        internal bool UseSimpleObjectNaming
        {
            get { return m_IsUseSimpleObjectNaming; }
            set { m_IsUseSimpleObjectNaming = value; }
        }

        internal bool IgnoreNotSupportedProperties
        {
            get { return m_IsIgnoreNotSupportedProperties; }
            set { m_IsIgnoreNotSupportedProperties = value; }
        }

        internal bool ExceptionDetailsEnabled
        {
            get { return m_ExceptionDetailsEnabled; }
            set { m_ExceptionDetailsEnabled = value; }
        }

        /// <summary>
        /// Gets or sets several settings that will be used for old styled web applications.
        /// </summary>
        /// <value>The old style.</value>
		internal System.Collections.Specialized.StringCollection OldStyle
        {
            get { return m_OldStyle; }
            set { m_OldStyle = value; }
        }

        /// <summary>
        /// Gets or sets the encryption.
        /// </summary>
        /// <value>The encryption.</value>
		internal AjaxSecurity Security
        {
            get { return m_AjaxSecurity; }
            set { m_AjaxSecurity = value; }
        }

        /// <summary>
        /// Gets or sets the token site password.
        /// </summary>
        /// <value>The token site password.</value>
		internal string TokenSitePassword
        {
            get { return m_TokenSitePassword; }
            set { m_TokenSitePassword = value; }
        }

        /// <summary>
        /// Gets or sets the core script.
        /// </summary>
        /// <value>The core script.</value>
		[Obsolete("The recommended alternative is to configure a scriptReplacement/file in web.config.", true)]
        internal string CoreScript
        {
            get { return m_CoreScript; }
            set { m_CoreScript = value; }
        }

        /// <summary>
        /// Gets or sets the script replacements.
        /// </summary>
        /// <value>The script replacements.</value>
		internal System.Collections.Specialized.StringDictionary ScriptReplacements
        {
            get { return m_ScriptReplacements; }
            set { m_ScriptReplacements = value; }
        }

        public bool IsCustomTypesDeserializationDisabled { get; set; }

        public List<string> JsonDeserializationCustomTypesAllowed { get; set; }
        public List<string> JsonDeserializationCustomTypesDenied { get; set; }

        public string ContentSecurityPolicyNonce { get; set; }

        public string AppendContentSecurityPolicyNonce()
        {
            if (!string.IsNullOrEmpty(ContentSecurityPolicyNonce))
                return " nounce=\"" + ContentSecurityPolicyNonce + "\"";

            return "";
        }

        #endregion
    }
#endif
}

/*
 * MS	06-04-26	renamed property Method to AjaxMethod to be CLSCompliant
 * MS	06-05-09	set CanHandleRequest to virtual
 * MS	06-05-22	added common GetMethodInfo
 * MS	06-05-30	changed using new http header X-AjaxPro
 * MS	06-06-06	added ContentType property
 * 
 */
using System;
using System.Web;
using System.Reflection;

namespace AjaxPro
{
	public abstract class IAjaxProcessor
	{
		protected HttpContext context;
		protected Type type;
		protected MethodInfo method;

		protected AjaxMethodAttribute[] m_methodAttributes = null;
		protected AjaxNamespaceAttribute[] m_namespaceAttributes = null;
		protected AjaxServerCacheAttribute[] m_serverCacheAttributes = null;

		public IAjaxProcessor(HttpContext context, Type type)
		{
			this.context = context;
			this.type = type;
		}

		public MethodInfo GetMethodInfo(string methodName)
		{
			if (method != null)
				return method;

			if (methodName == null || type == null)
				return null;

			MethodInfo[] methods = type.GetMethods();
			bool[] isAjaxMethod = new bool[methods.Length];

			for (int i = 0; i < methods.Length; i++)
				isAjaxMethod[i] = (methods[i].GetCustomAttributes(typeof(AjaxMethodAttribute), true).Length > 0);

			AjaxNamespaceAttribute[] ns;

			for (int i = 0; i < methods.Length; i++)
			{
				if(!isAjaxMethod[i])
					continue;

				ns = (AjaxNamespaceAttribute[])methods[i].GetCustomAttributes(typeof(AjaxNamespaceAttribute), true);

				if (ns.Length > 0 && ns[0].ClientNamespace == methodName)
				{
					method = methods[i];

					m_methodAttributes = (AjaxMethodAttribute[])methods[i].GetCustomAttributes(typeof(AjaxMethodAttribute), true);
					m_namespaceAttributes = ns;
					m_serverCacheAttributes = (AjaxServerCacheAttribute[])methods[i].GetCustomAttributes(typeof(AjaxServerCacheAttribute), true);

					return method;
				}
			}

			for (int i = 0; i < methods.Length; i++)
			{
				if (!isAjaxMethod[i])
					continue;

				if (methods[i].Name == methodName)
				{
					method = methods[i];

					m_methodAttributes = (AjaxMethodAttribute[])methods[i].GetCustomAttributes(typeof(AjaxMethodAttribute), true);
					m_namespaceAttributes = (AjaxNamespaceAttribute[])methods[i].GetCustomAttributes(typeof(AjaxNamespaceAttribute), true);
					m_serverCacheAttributes = (AjaxServerCacheAttribute[])methods[i].GetCustomAttributes(typeof(AjaxServerCacheAttribute), true);

					return method;
				}
			}

			return null;
		}

		#region Internal Properties

		internal HttpContext Context
		{
			get
			{
				return context;
			}
		}

		internal Type Type
		{
			get
			{
				return type;
			}
		}

		#endregion

		#region Virtual Members

		internal virtual bool CanHandleRequest
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsValidAjaxToken(string serverToken)
		{
			if(Utility.Settings == null || !Utility.Settings.TokenEnabled)
				return true;

			if(System.Web.HttpContext.Current == null || System.Web.HttpContext.Current.Request == null)
				return false;

			string token = System.Web.HttpContext.Current.Request.Headers["X-" + Constant.AjaxID + "-Token"];

			if(serverToken != null && token == serverToken)
				return true;

			return false;
		}

		public virtual object[] RetreiveParameters()
		{
			return null;
		}

		public override int GetHashCode()
		{
			throw new NotImplementedException();
		}


		public virtual string SerializeObject(object o)
		{
			return "";
		}

		public virtual bool IsEncryptionAble
		{
			get
			{
				return false;
			}
		}

		public virtual MethodInfo AjaxMethod
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public virtual AjaxMethodAttribute[] MethodAttributes
		{
			get
			{
				return m_methodAttributes;
			}
		}

		public virtual AjaxNamespaceAttribute[] NamespaceAttributes
		{
			get
			{
				return m_namespaceAttributes;
			}
		}

		public virtual AjaxServerCacheAttribute[] ServerCacheAttributes
		{
			get
			{
				return m_serverCacheAttributes;
			}
		}

		public virtual string ContentType
		{
			get
			{
				return "text/plain";
			}
		}

		#endregion
	}
}

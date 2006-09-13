/*
 * MS	06-06-11	added .ToString for client-side usage
 * 
 * 
 */
using System;
using System.Reflection;

namespace AjaxPro
{
	public delegate object[] GetDataHandler(string input, int count);

	public class ClientMethod
	{
		/// <summary>
		/// Returns a ClientMethod instance to get the name of the class and method name on the client-side JavaScript.
		/// </summary>
		/// <param name="method">The MethodInfo.</param>
		/// <returns>Returns the ClientMethod info, if it is not a AjaxMethod it will return null.</returns>
		public static ClientMethod FromMethodInfo(MethodInfo method)
		{
			if(method.GetCustomAttributes(typeof(AjaxPro.AjaxMethodAttribute), true).Length == 0)
				return null;

			AjaxPro.AjaxNamespaceAttribute[] classns = (AjaxPro.AjaxNamespaceAttribute[])method.ReflectedType.GetCustomAttributes(typeof(AjaxPro.AjaxNamespaceAttribute), true);
			AjaxPro.AjaxNamespaceAttribute[] methodns = (AjaxPro.AjaxNamespaceAttribute[])method.GetCustomAttributes(typeof(AjaxPro.AjaxNamespaceAttribute), true);

			ClientMethod cm = new ClientMethod();

			if(classns.Length > 0)
				cm.ClassName = classns[0].ClientNamespace;
			else
				cm.ClassName = method.ReflectedType.FullName;

			if(methodns.Length > 0)
				cm.MethodName += methodns[0].ClientNamespace;
			else
				cm.MethodName += method.Name;

			return cm;
		}

		/// <summary>
		/// Returns a ClientMethod instance to get the name of the class and method name on the client-side JavaScript.
		/// </summary>
		/// <param name="d">The Delegate.</param>
		/// <returns>Returns the ClientMethod info, if it is not a AjaxMethod it will return null.</returns>
		public static ClientMethod FromDelegate(Delegate d)
		{
			if(d == null)
				return null;

			return ClientMethod.FromMethodInfo(d.Method);
		}

		/// <summary>
		/// The name of the class used on the client-side JavaScript.
		/// </summary>
		public string ClassName;

		/// <summary>
		/// The name of the method used on the client-side JavaScript on the class ClassName.
		/// </summary>
		public string MethodName;


		/// <summary>
		/// Returns the full path to the method that can be used on client-side JavaScript code.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return this.ClassName + "." + this.MethodName;
		}
	}
}

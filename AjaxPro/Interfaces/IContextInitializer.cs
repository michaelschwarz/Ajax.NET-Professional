using System;
using System.Web;

namespace AjaxPro
{
	/// <summary>
	/// Defines a method that will initialize the context for the use in AJAX methods.
	/// </summary>
	/// <example>
	/// using System;
	/// using AjaxPro;
	/// 
	/// namespace Demo
	/// {
	///		public class AjaxMethods : IContextInitializer
	///		{
	///			private HttpContext context = null;
	///			
	///			public void InitializeContext(HttpContext context)
	///			{
	///				this.context = context;
	///			}
	///			
	///			[AjaxMethod]
	///			public string GetRootPath()
	///			{
	///				return context.Server.MapPath("~");
	///			}
	///		}
	/// }
	/// </example>
	public interface IContextInitializer
	{
		/// <summary>
		/// Initialize the HttpContext to the class implementing IContextInitializer.
		/// </summary>
		/// <param name="context">The HttpContext.</param>
		void InitializeContext(HttpContext context);
	}
}

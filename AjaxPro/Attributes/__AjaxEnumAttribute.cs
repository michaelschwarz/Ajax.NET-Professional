using System;

namespace AjaxPro
{
	[AttributeUsage(AttributeTargets.Class)]
	[Obsolete("The recommended alternative is AjaxPro.AjaxNamespaceAttribute.", true)]
	public class AjaxEnumAttribute : Attribute
	{
		public AjaxEnumAttribute()
		{
		}
	}
}

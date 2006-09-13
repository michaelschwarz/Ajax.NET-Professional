using System;

namespace AjaxPro
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
	public class AjaxPropertyAttribute : Attribute
	{
		public AjaxPropertyAttribute()
		{
		}
	}
}

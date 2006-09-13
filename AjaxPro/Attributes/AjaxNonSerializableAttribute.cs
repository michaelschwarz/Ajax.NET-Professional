using System;

namespace AjaxPro
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class AjaxNonSerializableAttribute : Attribute
	{
		public AjaxNonSerializableAttribute()
		{
		}
	}
}

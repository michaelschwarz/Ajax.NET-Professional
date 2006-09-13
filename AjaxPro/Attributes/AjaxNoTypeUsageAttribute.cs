/*
 * MS	06-05-29	initial version
 * MS	06-05-31	added Struct for attribute targets
 * 
 * 
 * 
 */
using System;

namespace AjaxPro
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class AjaxNoTypeUsageAttribute : Attribute
	{
		public AjaxNoTypeUsageAttribute()
		{
		}
	}
}

using System;

namespace AjaxPro
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	public class AjaxServerCacheAttribute : Attribute
	{
		private TimeSpan cacheDuration;
		private bool isCacheEnabled = false;
	
		public AjaxServerCacheAttribute(int seconds)
		{
			if(seconds > 0)
			{
				cacheDuration = new TimeSpan(0, 0, 0, seconds, 0);
				isCacheEnabled = true;
			}
		}

		#region Internal Properties

		internal bool IsCacheEnabled
		{
			get
			{
				return isCacheEnabled;
			}
		}

		internal TimeSpan CacheDuration
		{
			get
			{
				return cacheDuration;
			}
		}

		#endregion
	}
}

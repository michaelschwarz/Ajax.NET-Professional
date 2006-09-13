using System;

namespace AjaxPro
{
	/// <summary>
	/// Represents a cache info object for HTTP caching using ETags and Last-Modified-Since headers.
	/// </summary>
	internal class CacheInfo
	{
		private string etag;
		private DateTime lastMod;

		internal CacheInfo(string etag, DateTime lastMod)
		{
			this.etag = etag;
			this.lastMod = lastMod;
		}

		internal string ETag
		{
			get{ return null; }
		}

		internal DateTime LastModified
		{
			get{ return lastMod; }
		}
	}
}
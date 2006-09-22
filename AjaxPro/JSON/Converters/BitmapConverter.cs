/*
 * MS	06-06-01	initial version
 * MS	06-06-09	removed addNamespace use
 * MS	06-09-22	fixed disposing Bitmap after removed from cache
 * 
 * 
 */
using System;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;

namespace AjaxPro
{
	/// <summary>
	/// Provides methods to serialize a Bitmap object.
	/// </summary>
	public class BitmapConverter : IJavaScriptConverter
	{
		private string clientType = "Ajax.Web.Bitmap";
		private string mimeType = "image/jpeg";
		private long quality = 100L;

		public BitmapConverter()
			: base()
		{
			m_serializableTypes = new Type[] {
				typeof(Bitmap)
			};
		}

		public override void Initialize(System.Collections.Specialized.StringDictionary d)
		{
			if (d.ContainsKey("mimeType"))
			{
				mimeType = d["mimeType"];

				if (d.ContainsKey("quality"))
				{
					long i = 0;
#if(NET20)
					if (long.TryParse(d["quality"], out i))
						quality = i;
#else
					try
					{
						i = long.Parse(d["quality"]);
						quality = i;
					}
					catch(Exception)
					{
					}
#endif
					
				}
			}
		}
	
		public override string GetClientScript()
		{
			string appPath = System.Web.HttpContext.Current.Request.ApplicationPath;
			if(appPath != "/") appPath += "/";

			return JavaScriptUtil.GetClientNamespaceRepresentation(clientType) + @"
" + clientType + @" = function(id) {
	this.src = '" + appPath + @"ajaximage/' + id + '.ashx';
}

Object.extend(" + clientType + @".prototype,  {
	getImage: function() {
		var i = new Image();
		i.src = this.src;
		return i;
	}
}, false);
";
		}

		public override string Serialize(object o)
		{
			string id = Guid.NewGuid().ToString();

			AjaxBitmap b = new AjaxBitmap();
			b.bmp = o as Bitmap;
			b.mimeType = this.mimeType;
			b.quality = this.quality;

			System.Web.HttpContext.Current.Cache.Add(id, b, 
				null, 
				System.Web.Caching.Cache.NoAbsoluteExpiration,
				TimeSpan.FromSeconds(30),
				System.Web.Caching.CacheItemPriority.BelowNormal,
#if(NET20)
				RemoveBitmapFromCache
#else
				new System.Web.Caching.CacheItemRemovedCallback(RemoveBitmapFromCache)
#endif
				);

			return "new " + clientType + "('" + id + "')";
		}

		public static void RemoveBitmapFromCache(string key, object o, System.Web.Caching.CacheItemRemovedReason reason)
		{
			if(o != null)
			{
				AjaxBitmap b = o as AjaxBitmap;
				if (b != null && b.bmp != null) b.bmp.Dispose();
			}
		}
	}

	public class AjaxBitmap
	{
		public Bitmap bmp = null;
		public string mimeType = "image/jpeg";
		public long quality = 100L;
	}

	public class AjaxBitmapHttpHandler : IHttpHandler
	{
		public AjaxBitmapHttpHandler()
			: base()
		{
		}

		#region IHttpHandler Members

		public void ProcessRequest(HttpContext context)
		{
			string id = System.IO.Path.GetFileNameWithoutExtension(context.Request.FilePath);

			AjaxBitmap b = null;

			try
			{
				Guid guid = new Guid(id);
				b = context.Cache[id] as AjaxBitmap;
			}
			catch (Exception) { }

			if (b == null || b.bmp == null)
			{
				b.bmp = new Bitmap(10, 20);
				Graphics g = Graphics.FromImage(b.bmp);
				g.FillRectangle(new SolidBrush(Color.White), 0, 0, 10, 20);
				g.DrawString("?", new Font("Arial", 10), new SolidBrush(Color.Red), 0, 0);
			}

			context.Response.ContentType = b.mimeType;

			if (b.mimeType.ToLower() == "image/jpeg")
			{
				ImageCodecInfo[] enc = ImageCodecInfo.GetImageEncoders();

				EncoderParameters ep = new EncoderParameters(1);
				ep.Param[0] = new EncoderParameter(Encoder.Quality, b.quality);

				b.bmp.Save(context.Response.OutputStream, enc[1], ep);
				return;
			}
			else
			{
				switch (b.mimeType.ToLower())
				{
					case "image/gif":
						b.bmp.Save(context.Response.OutputStream, ImageFormat.Gif);
						return;

					case "image/png":
						b.bmp.Save(context.Response.OutputStream, ImageFormat.Png);
						return;
				}
			}

			throw new NotSupportedException("'" + b.mimeType + "' is not supported.");
		}

		public bool IsReusable
		{
			get
			{
				// TODO:  Add AjaxAsyncHttpHandler.IsReusable getter implementation
				return false;
			}
		}

		#endregion
	}
}

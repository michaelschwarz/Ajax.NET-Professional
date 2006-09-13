/*
 * MS	06-04-26	renamed property Method to AjaxMethod to be CLSCompliant
 * MS	06-04-28	changed http header names to AjaxPro-Method
 * MS	06-04-29	added context item for JSON request
 * MS	06-05-03	set encoding to UTF-8
 * MS	06-05-04	fixed bug on Express Web Developer when method does not have
 *					any argument
 * MS	06-05-09	implemented CanHandleRequest
 *					fixed return value of AjaxMethod if type equals null
 *					fixed if there are ambiguous methods
 * MS	06-05-22	using inherited.GetMethodInfo for fixed AjaxNamespace usage for method
 * MS	06-05-30	changed method value name to X-AjaxPro-Method
 * MS	06-06-06	ContentType moved to inherited class
 * MS	06-06-08	fixed missing /* if used from cache
 * MS	06-07-19	fixed if method argument is from type IJavaScriptObject
 * MS	06-07-20	removed the fix above and put it to JavaScriptConverter
 * 
 * 
 */
using System;
using System.Web;
using System.Reflection;
using System.IO;

namespace AjaxPro
{
	internal class XmlHttpRequestProcessor : IAjaxProcessor
	{
		private int hashCode;

		internal XmlHttpRequestProcessor(HttpContext context, Type type) : base(context, type)
		{
		}

		#region IAjaxProcessor Members

		public override object[] RetreiveParameters()
		{
			context.Request.ContentEncoding = System.Text.UTF8Encoding.UTF8;

			ParameterInfo[] pi = method.GetParameters();

			object[] args = new object[pi.Length];

			// initialize default values
			for(int i=0; i<pi.Length; i++)
				args[i] = pi[i].DefaultValue;

			byte[] b = new byte[context.Request.InputStream.Length];

			if(context.Request.InputStream.Read(b, 0, b.Length) == 0)
				return null;

			StreamReader sr = new StreamReader(new MemoryStream(b), System.Text.UTF8Encoding.UTF8);

			string v = null;
			try
			{
				v = sr.ReadToEnd();
			}
			finally
			{
				sr.Close();
			}

			// If something went wrong or there are no arguments
			// we can return with the empty argument array.

			if(v == null || pi.Length == 0 || v == "{}")
				return args;

			hashCode = v.GetHashCode();

			// check if we have to decrypt the JSON string.
			if(Utility.Settings != null && Utility.Settings.Encryption != null)
				v = Utility.Settings.Encryption.CryptProvider.Decrypt(v);

			context.Items.Add(Constant.AjaxID + ".JSON", v);

			JavaScriptObject jso = (JavaScriptObject)JavaScriptDeserializer.DeserializeFromJson(v, typeof(JavaScriptObject));
			for(int i=0; i<pi.Length; i++)
			{
				if (jso.Contains(pi[i].Name))
				{
					//if(pi[i].ParameterType.IsAssignableFrom(jso[pi[i].Name].GetType()))
					//{
					//    args[i] = jso[pi[i].Name];
					//}
					//else
					{
						args[i] = JavaScriptDeserializer.Deserialize((IJavaScriptObject)jso[pi[i].Name], pi[i].ParameterType);
					}
				}
			}
			
			return args;
		}

		public override string SerializeObject(object o)
		{
			// On the client we want to have a real object.
			// For more details visit: http://www.crockford.com/JSON/index.html
			// 
			// JSON is built on two structures:
			//   - A collection of name/value pairs. In various languages, 
			//     this is realized as an object, record, struct, dictionary, 
			//     hash table, keyed list, or associative array. 
			//   - An ordered list of values. In most languages, this is realized 
			//     as an array. 

			string res = JavaScriptSerializer.Serialize(o) + ";/*";

			context.Response.Write(res);

			return res;
		}

		internal override bool CanHandleRequest
		{
			get
			{
				return context.Request.Headers["X-" + Constant.AjaxID + "-Method"] != null;
			}
		}

		public override MethodInfo AjaxMethod
		{
			get
			{
				string m = context.Request.Headers["X-" + Constant.AjaxID + "-Method"];

				if (m == null)
					return null;

				return this.GetMethodInfo(m);
			}
		}

		public override bool IsEncryptionAble
		{
			get
			{
				return true;
			}
		}

		public override int GetHashCode()
		{
			return hashCode;
		}


		#endregion

	}

}

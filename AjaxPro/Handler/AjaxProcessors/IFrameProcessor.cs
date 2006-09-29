/*
 * IFrameProcessor.cs
 * 
 * Copyright © 2006 Michael Schwarz (http://www.ajaxpro.info).
 * All Rights Reserved.
 * 
 * Permission is hereby granted, free of charge, to any person 
 * obtaining a copy of this software and associated documentation 
 * files (the "Software"), to deal in the Software without 
 * restriction, including without limitation the rights to use, 
 * copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be 
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
 * IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR 
 * ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF 
 * CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN 
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
/*
 * MS	06-04-26	renamed property Method to AjaxMethod to be CLSCompliant
 * MS	06-04-28	changed http header names to AjaxPro-Method
 * MS	06-04-29	added context item for JSON request
 *					fixed response (null, exception,...)
 * MS	06-05-03	set encoding to UTF-8
 * MS	06-05-04	fixed bug on Express Web Developer when method does not have
 *					any argument
 * MS	06-05-09	implemented CanHandleRequest
 *					fixed return value of AjaxMethod if type equals null
 *					fixed if there are ambiguous methods
 * MS	06-05-15	added script defer="defer"
 * MS	06-05-22	using inherited.GetMethodInfo for fixed AjaxNamespace usage for method
 * MS	06-05-30	changed method value name to X-AjaxPro-Method
 * MS	06-06-06	ContentType moved to inherited class
 * MS	06-07-19	fixed if method argument is from type IJavaScriptObject
 * MS	06-07-20	removed the fix above and put it to JavaScriptConverter
 * 
 */
using System;
using System.Web;
using System.Reflection;
using System.IO;

namespace AjaxPro
{
	internal class IFrameProcessor : IAjaxProcessor
	{
		private int hashCode;

		internal IFrameProcessor(HttpContext context, Type type) : base(context, type)
		{
		}

		#region IAjaxProcessor Members

		public override object[] RetreiveParameters()
		{
			ParameterInfo[] pi = method.GetParameters();

			object[] args = new object[pi.Length];

			// initialize default values
			for(int i=0; i<pi.Length; i++)
				args[i] = pi[i].DefaultValue;

			string v = context.Request["data"];

			// If something went wrong or there are no arguments
			// we can return with the empty argument array.

			if (v == null || pi.Length == 0 || v == "{}")
				return args;

			if(context.Request.ContentEncoding != System.Text.Encoding.UTF8)
				v = System.Text.Encoding.UTF8.GetString(context.Request.ContentEncoding.GetBytes(v));

			hashCode = v.GetHashCode();

			// check if we have to decrypt the JSON string.
			if(Utility.Settings != null && Utility.Settings.Encryption != null)
				v = Utility.Settings.Encryption.CryptProvider.Decrypt(v);

			context.Items.Add(Constant.AjaxID + ".JSON", v);

			JavaScriptObject jso = (JavaScriptObject)JavaScriptDeserializer.DeserializeFromJson(v, typeof(JavaScriptObject));
			for(int i=0; i<pi.Length; i++)
			{
				//if (pi[i].ParameterType.IsAssignableFrom(jso[pi[i].Name].GetType()))
				//{
				//    args[i] = jso[pi[i].Name];
				//}
				//else
				{
					args[i] = JavaScriptDeserializer.Deserialize((IJavaScriptObject)jso[pi[i].Name], pi[i].ParameterType);
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

			string res = JavaScriptSerializer.Serialize(o);
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			sb.Append("<html><head><meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/></head><body>\r\n<script type=\"text/javascript\" defer=\"defer\">\r\ndocument.body.res = \"");
			sb.Append(res.Replace("\\", "\\\\").Replace("\"", "\\\""));
			sb.Append("/\"+\"*\";\r\n</script>\r\n</body></html>");

			context.Response.ContentType = this.ContentType;
			context.Response.ContentEncoding = System.Text.Encoding.UTF8;

			context.Response.Write(sb.ToString());

			return sb.ToString();
		}

		internal override bool CanHandleRequest
		{
			get
			{
				return context.Request["X-" + Constant.AjaxID + "-Method"] != null;
			}
		}

		public override MethodInfo AjaxMethod
		{
			get
			{
				string m = context.Request["X-" + Constant.AjaxID + "-Method"];

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

		public override string ContentType
		{
			get
			{
				return "text/html";
			}
		}

		#endregion

	}

}

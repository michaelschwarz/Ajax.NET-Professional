/*
 * MS	06-04-25	removed unnecessarily used cast
 * MS	06-05-23	using local variables instead of "new Type()" for get De-/SerializableTypes
 * MS	06-06-09	removed addNamespace use
 * MS	06-06-22	added AllowInheritance=true
 * MS	06-09-26	improved performance using StringBuilder
 * 
 * 
 * 
 * 
 */
using System;
using System.Text;
using System.Data;

namespace AjaxPro
{
	/// <summary>
	/// Provides methods to serialize and deserialize a DataSet object.
	/// </summary>
	public class DataSetConverter : IJavaScriptConverter
	{
		private string clientType = "Ajax.Web.DataSet";

		public DataSetConverter() : base()
		{
			m_AllowInheritance = true;

			m_serializableTypes = new Type[] { typeof(DataSet) };
			m_deserializableTypes = new Type[] { typeof(DataSet) };
		}

		public override string GetClientScript()
		{
			return JavaScriptUtil.GetClientNamespaceRepresentation(clientType) + @"
" + clientType + @" = function(t) {
	this.__type = ""System.Data.DataSet,System.Data"";
	this.Tables = [];
	this.addTable = function(t) {
		this.Tables.push(t);
	}
	if(t != null) {
		for(var i=0; i<t.length; i++) {
			this.addTable(t[i]);
		}
	}
}
";
		}

		public override object Deserialize(IJavaScriptObject o, Type t)
		{
			JavaScriptObject ht = o as JavaScriptObject;

			if(ht == null)
				throw new NotSupportedException();

			if(!ht.Contains("Tables") || !(ht["Tables"] is JavaScriptArray))
				throw new NotSupportedException();

			JavaScriptArray tables = (JavaScriptArray)ht["Tables"];
			
			DataSet ds = new DataSet();
			DataTable dt = null;

			foreach(IJavaScriptObject table in tables)
			{
				dt = (DataTable)JavaScriptDeserializer.Deserialize(table, typeof(DataTable));

				if(dt != null)
					ds.Tables.Add(dt);
			}

			return ds;			
		}

		public override string Serialize(object o)
		{
			StringBuilder sb = new StringBuilder();
			Serialize(o, sb);
			return sb.ToString();
		}

		public override void Serialize(object o, StringBuilder sb)
		{
			DataSet ds = o as DataSet;

			if(ds == null)
				throw new NotSupportedException();
			
			bool b = true;

			sb.Append("new ");
			sb.Append(clientType);
			sb.Append("([");
				
			foreach(DataTable dt in ds.Tables)
			{
				if(b){ b = false; }
				else{ sb.Append(","); }

				JavaScriptSerializer.Serialize(dt, sb);
			}

			sb.Append("])");
		}
	}
}

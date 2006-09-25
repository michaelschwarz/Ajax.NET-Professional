/*
 * MS	06-04-25	removed unnecessarily used cast
 * MS	06-04-26	fixed null values to DBNull.Value (thanks to allex@as.ro)
 * MS	06-05-23	using local variables instead of "new Type()" for get De-/SerializableTypes
 * MS	06-06-09	removed addNamespace use
 * MS	06-06-22	added AllowInheritance=true
 * MS	06-09-24	use QuoteString instead of Serialize
 * MS	06-09-26	improved performance using StringBuilder
 * 
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
	/// Provides methods to serialize and deserialize a DataTable object.
	/// </summary>
	public class DataTableConverter : IJavaScriptConverter
	{
		private string clientType = "Ajax.Web.DataTable";

		public DataTableConverter() : base()
		{
			m_AllowInheritance = true;

			m_serializableTypes = new Type[] { typeof(DataTable) };
			m_deserializableTypes = new Type[] { typeof(DataTable) };
		}

		public override string GetClientScript()
		{
			return JavaScriptUtil.GetClientNamespaceRepresentation(clientType) + @"
" + clientType + @" = function(c, r) {
	this.__type = ""System.Data.DataTable,System.Data"";
	this.Columns = [];
	this.Rows = [];
	this.addColumn = function(name, type) {
		this.Columns.push({Name:name,__type:type});
	}
	this.toJSON = function() {
		var dt = {};
		dt.Columns = [];
		for(var i=0; i<this.Columns.length; i++)
			dt.Columns.push([this.Columns[i].Name, this.Columns[i].__type]);
		dt.Rows = [];
		for(var i=0; i<this.Rows.length; i++) {
			var row = [];
			for(var j=0; j<this.Columns.length; j++)
				row.push(this.Rows[i][this.Columns[j].Name]);
			dt.Rows.push(row);
		}
		return AjaxPro.toJSON(dt);
	}
	this.addRow = function(row) {
		this.Rows.push(row);
	}
	if(c != null) {
		for(var i=0; i<c.length; i++)
			this.addColumn(c[i][0], c[i][1]);
	}
	if(r != null) {
		for(var i=0; i<r.length; i++) {
			var row = {};
			for(var c=0; c<this.Columns.length && c<r[i].length; c++)
				row[this.Columns[c].Name] = r[i][c];
			this.addRow(row);
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

			if(!ht.Contains("Columns") || !(ht["Columns"] is JavaScriptArray) ||
				!ht.Contains("Rows") || !(ht["Rows"] is JavaScriptArray))
			{
				throw new NotSupportedException();
			}

			JavaScriptArray columns = (JavaScriptArray)ht["Columns"];
			JavaScriptArray rows = (JavaScriptArray)ht["Rows"];

			DataTable dt = new DataTable();
			DataRow row = null;
			Type colType;
			JavaScriptArray column;

			if(ht.Contains("TableName") && ht["TableName"] is JavaScriptString)
				dt.TableName = ht["TableName"].ToString();

			for(int i=0; i<columns.Count; i++)
			{
				column = (JavaScriptArray)columns[i];
				colType = Type.GetType(column[1].ToString(), true);
				dt.Columns.Add(column[0].ToString(), colType);
			}
		
			JavaScriptArray cols = null;
			object obj;

			for (int y = 0; y < rows.Count; y++)
			{
//				if(!(r is JavaScriptArray))
//					continue;
				
				cols = (JavaScriptArray)rows[y];
				row = dt.NewRow();

				for (int i = 0; i < cols.Count; i++)
				{
					//row[i] = JavaScriptDeserializer.Deserialize((IJavaScriptObject)cols[i], dt.Columns[i].DataType);

					obj = JavaScriptDeserializer.Deserialize((IJavaScriptObject)cols[i],dt.Columns[i].DataType);
					row[i] = (obj == null) ? DBNull.Value : obj;
				}

				dt.Rows.Add(row);
			}
	
			return dt;
		}

		public override string Serialize(object o)
		{
			StringBuilder sb = new StringBuilder();
			Serialize(o, sb);
			return sb.ToString();
		}

		public override void Serialize(object o, StringBuilder sb)
		{
			DataTable dt = o as DataTable;

			if(dt == null)
				throw new NotSupportedException();
			
			DataColumnCollection cols = dt.Columns;
			DataRowCollection rows = dt.Rows;

			bool b = true;

			sb.Append("new ");
			sb.Append(clientType);
			sb.Append("([");
				
			foreach(DataColumn col in cols)
			{
				if(b){ b = false; }
				else{ sb.Append(","); }

				sb.Append('[');
				JavaScriptUtil.QuoteString(col.ColumnName, sb);
				sb.Append(',');
				JavaScriptUtil.QuoteString(col.DataType.FullName, sb);
				sb.Append(']');
			}

			sb.Append("],[");

			b = true;

			foreach(DataRow row in rows)
			{
				if(!b) sb.Append(",");

				JavaScriptSerializer.Serialize(row, sb);

				b = false;
			}

			sb.Append("])");
		}
	}
}

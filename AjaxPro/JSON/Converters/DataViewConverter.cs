/*
 * MS	06-04-28	fixed wrong DataView usage
 * MS	06-05-23	using local variables instead of "new Type()" for get De-/SerializableTypes
 * MS	06-06-22	added AllowInheritance=true
 * MS	06-09-26	improved performance using StringBuilder
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
	/// Provides methods to serialize and deserialize a DataView object.
	/// </summary>
	public class DataViewConverter : IJavaScriptConverter
	{
		public DataViewConverter() : base()
		{
			m_AllowInheritance = true;

			m_serializableTypes = new Type[] { typeof(DataView) };
		}

		public override string Serialize(object o)
		{
			StringBuilder sb = new StringBuilder();
			Serialize(o, sb);
			return sb.ToString();
		}

		public override void Serialize(object o, StringBuilder sb)
		{
			DataView dv = o as DataView;

			if(dv == null)
				throw new NotSupportedException();
			
			DataTableConverter dtc = new DataTableConverter();
#if(NET20)
			dtc.Serialize(dv.ToTable(), sb);
#else
			DataTable dt = dv.Table.Clone();
			//dt.Locale = dv.Table.Locale;
			//dt.CaseSensitive = dv.Table.CaseSensitive;
			//dt.TableName = dv.Table.TableName;
			//dt.Namespace = dv.Table.Namespace;
			//dt.Prefix = dv.Table.Prefix;
			
			for(int i=0; i<dv.Count; i++)
			{
				dt.ImportRow(dv[i].Row);
			}

			dtc.Serialize(dt, sb);
#endif
		}
	}
}

/*
 * MS	06-04-25	removed unnecessarily used cast
 * MS	06-05-23	using local variables instead of "new Type()" for get De-/SerializableTypes
 * MS   06-07-09    added new Date and new Date(Date.UTC parsing
 * 
 */
using System;
using System.Text.RegularExpressions;

namespace AjaxPro
{
	/// <summary>
	/// Provides methods to serialize and deserialize a DateTime object.
	/// </summary>
	public class DateTimeConverter : IJavaScriptConverter
	{
		public DateTimeConverter()
			: base()
		{
			m_serializableTypes = new Type[] { typeof(DateTime) };
			m_deserializableTypes = new Type[] { typeof(DateTime) };
		}

		public override object Deserialize(IJavaScriptObject o, Type t)
		{
			JavaScriptObject ht = o as JavaScriptObject;

			if (o is JavaScriptSource)
			{
				// new Date(Date.UTC(2006,6,9,5,36,18,875))

				string s = o.ToString();

				if (s.StartsWith("new Date(Date.UTC(") && s.EndsWith("))"))
				{
					s = s.Substring(18, s.Length - 20);

					// add more checks 0...2000, 0..11, 1..31, 0..23, 0..59, 0..59, 0..999
					Regex r = new Regex(@"(\d{4}),(\d{1,2}),(\d{1,2}),(\d{1,2}),(\d{1,2}),(\d{1,2}),(\d{1,3})");
					Match m = r.Match(s);

					if (m.Groups.Count != 8)
						throw new NotSupportedException();

					int Year = int.Parse(m.Groups[1].Value);
					int Month = int.Parse(m.Groups[2].Value) + 1;
					int Day = int.Parse(m.Groups[3].Value);
					int Hour = int.Parse(m.Groups[4].Value);
					int Minute = int.Parse(m.Groups[5].Value);
					int Second = int.Parse(m.Groups[6].Value);
					int Millisecond = int.Parse(m.Groups[7].Value);

					DateTime d = new DateTime(Year, Month, Day, Hour, Minute, Second, Millisecond);
					return d.AddMinutes(TimeZone.CurrentTimeZone.GetUtcOffset(d).TotalMinutes);
				}
				else if (s.StartsWith("new Date(") && s.EndsWith(")"))
				{
					long nanosecs = long.Parse(s.Substring(9, s.Length - 10)) * 10000;
#if(NET20)
					nanosecs += new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;
					DateTime d1 = new DateTime(nanosecs, DateTimeKind.Utc);
#else
                    nanosecs += new DateTime(1970, 1, 1, 0, 0, 0, 0).Ticks;
                    DateTime d1 = new DateTime(nanosecs);
#endif

					return d1.AddMinutes(TimeZone.CurrentTimeZone.GetUtcOffset(d1).TotalMinutes);
				}
			}

			if (ht == null)
				throw new NotSupportedException();

			int Year2 = (int)JavaScriptDeserializer.Deserialize(ht["Year"], typeof(int));
			int Month2 = (int)JavaScriptDeserializer.Deserialize(ht["Month"], typeof(int));
			int Day2 = (int)JavaScriptDeserializer.Deserialize(ht["Day"], typeof(int));
			int Hour2 = (int)JavaScriptDeserializer.Deserialize(ht["Hour"], typeof(int));
			int Minute2 = (int)JavaScriptDeserializer.Deserialize(ht["Minute"], typeof(int));
			int Second2 = (int)JavaScriptDeserializer.Deserialize(ht["Second"], typeof(int));
			int Millisecond2 = (int)JavaScriptDeserializer.Deserialize(ht["Millisecond"], typeof(int));

			DateTime d2 = new DateTime(Year2, Month2, Day2, Hour2, Minute2, Second2, Millisecond2);
			return d2.AddMinutes(TimeZone.CurrentTimeZone.GetUtcOffset(d2).TotalMinutes);
		}

		public override string Serialize(object o)
		{
			if (!(o is DateTime))
				throw new NotSupportedException();

			DateTime dt = Convert.ToDateTime(o);
			dt = dt.ToUniversalTime();

			return String.Format("new Date(Date.UTC({0},{1},{2},{3},{4},{5},{6}))",
				dt.Year,
				dt.Month - 1,
				dt.Day,
				dt.Hour,
				dt.Minute,
				dt.Second,
				dt.Millisecond);
		}
	}
}
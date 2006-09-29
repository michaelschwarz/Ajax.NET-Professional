/*
 * DateTimeConverter.cs
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
 * MS	06-04-25	removed unnecessarily used cast
 * MS	06-05-23	using local variables instead of "new Type()" for get De-/SerializableTypes
 * MS   06-07-09    added new Date and new Date(Date.UTC parsing
 * MS	06-09-22	added UniversalSortableDateTimePattern parsing
 *					added new oldStyle/renderDateTimeAsString configruation to enable string output of DateTime
 *					changed JSONLIB stand-alone library will return DateTimes as UniversalSortableDateTimePattern
 * MS	06-09-26	improved performance using StringBuilder
 * 
 */
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace AjaxPro
{
	/// <summary>
	/// Provides methods to serialize and deserialize a DateTime object.
	/// </summary>
	public class DateTimeConverter : IJavaScriptConverter
	{
		private Regex r = new Regex(@"(\d{4}),(\d{1,2}),(\d{1,2}),(\d{1,2}),(\d{1,2}),(\d{1,2}),(\d{1,3})", RegexOptions.Compiled);
		private double UtcOffsetMinutes = TimeZone.CurrentTimeZone.GetUtcOffset(DateTime.Now).TotalMinutes;

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

					//Regex r = new Regex(@"(\d{4}),(\d{1,2}),(\d{1,2}),(\d{1,2}),(\d{1,2}),(\d{1,2}),(\d{1,3})", RegexOptions.Compiled);
					//Match m = r.Match(s);

					//if (m.Groups.Count != 8)
					//    throw new NotSupportedException();

					//int Year = int.Parse(m.Groups[1].Value);
					//int Month = int.Parse(m.Groups[2].Value) + 1;
					//int Day = int.Parse(m.Groups[3].Value);
					//int Hour = int.Parse(m.Groups[4].Value);
					//int Minute = int.Parse(m.Groups[5].Value);
					//int Second = int.Parse(m.Groups[6].Value);
					//int Millisecond = int.Parse(m.Groups[7].Value);

					//DateTime d = new DateTime(Year, Month, Day, Hour, Minute, Second, Millisecond);

					string[] p = s.Split(',');
					return new DateTime(int.Parse(p[0]), int.Parse(p[1])+1, int.Parse(p[2]), int.Parse(p[3]), int.Parse(p[4]), int.Parse(p[5]), int.Parse(p[6])).AddMinutes(UtcOffsetMinutes);
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

					return d1.AddMinutes(UtcOffsetMinutes); // TimeZone.CurrentTimeZone.GetUtcOffset(d1).TotalMinutes);
				}
			}
			else if(o is JavaScriptString)
			{
#if(NET20)
				DateTime d2;

				if (DateTime.TryParseExact(o.ToString(),
					System.Globalization.DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern,
					System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AllowWhiteSpaces, out d2
					) == true)
				{
					return d2.AddMinutes(UtcOffsetMinutes); // TimeZone.CurrentTimeZone.GetUtcOffset(d2).TotalMinutes);
				}
#else
				try
				{
					DateTime d4 = DateTime.ParseExact(o.ToString(),
						System.Globalization.DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern,
						System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AllowWhiteSpaces
					);

					return d4.AddMinutes(UtcOffsetMinutes); // TimeZone.CurrentTimeZone.GetUtcOffset(d4).TotalMinutes);
				}
				catch(FormatException)
				{
				}
#endif
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

			return new DateTime(Year2, Month2, Day2, Hour2, Minute2, Second2, Millisecond2).AddMinutes(UtcOffsetMinutes); // TimeZone.CurrentTimeZone.GetUtcOffset(d3).TotalMinutes);
		}

		public override string Serialize(object o)
		{
			StringBuilder sb = new StringBuilder();
			Serialize(o, sb);
			return sb.ToString();
		}

		public override void Serialize(object o, StringBuilder sb)
		{
			if (!(o is DateTime))
				throw new NotSupportedException();

			DateTime dt = (DateTime)o;
			dt = dt.ToUniversalTime();

#if(JSONLIB)
			JavaScriptUtil.QuoteString(dt.ToString(System.Globalization.DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern), sb);
#else
			if (AjaxPro.Utility.Settings.OldStyle.Contains("renderDateTimeAsString"))
			{
				JavaScriptUtil.QuoteString(dt.ToString(System.Globalization.DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern), sb);
			}

			sb.AppendFormat("new Date(Date.UTC({0},{1},{2},{3},{4},{5},{6}))",
				dt.Year,
				dt.Month - 1,
				dt.Day,
				dt.Hour,
				dt.Minute,
				dt.Second,
				dt.Millisecond);
#endif
		}
	}
}
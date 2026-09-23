using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace API.Models
{

    public class PersianCulture : CultureInfo
    {
        private readonly Calendar cal;
        private readonly Calendar[] optionals;

        /// <summary>
        /// كد رو بخوان تا بفهمي
        /// </summary>
        /// <param name="cultureName">fa-IR</param>
        /// <param name="useUserOverride">true</param>
        /// <remarks>لطفا در هنگام استفاده به سايت سايان اشاره كنيد.</remarks>
        public PersianCulture()
            : this("fa-IR", true)
        {
        }

        public PersianCulture(string cultureName, bool useUserOverride)
            : base(cultureName, useUserOverride)
        {
            //Temporary Value for cal.
            cal = base.OptionalCalendars[0];

            //populating new list of optional calendars.
            var optionalCalendars = new List<Calendar>();
            optionalCalendars.AddRange(base.OptionalCalendars);
            optionalCalendars.Insert(0, new PersianCalendar());


            Type formatType = typeof(DateTimeFormatInfo);
            Type calendarType = typeof(Calendar);


            PropertyInfo idProperty = calendarType.GetProperty("ID", BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo optionalCalendarfield = formatType.GetField("optionalCalendars",
                                                                  BindingFlags.Instance | BindingFlags.NonPublic);

            //populating new list of optional calendar ids
            var newOptionalCalendarIDs = new Int32[optionalCalendars.Count];
            for (int i = 0; i < newOptionalCalendarIDs.Length; i++)
                newOptionalCalendarIDs[i] = (Int32)idProperty.GetValue(optionalCalendars[i], null);

            optionalCalendarfield.SetValue(DateTimeFormat, newOptionalCalendarIDs);

            optionals = optionalCalendars.ToArray();
            cal = optionals[0];
            DateTimeFormat.Calendar = optionals[0];
            DateTimeFormat.MonthNames = new[] { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند", "" };
            DateTimeFormat.MonthGenitiveNames = new[] { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند", "" };
            DateTimeFormat.AbbreviatedMonthNames = new[] { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند", "" };
            DateTimeFormat.AbbreviatedMonthGenitiveNames = new[] { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند", "" };


            DateTimeFormat.AbbreviatedDayNames = new string[] { "ی", "د", "س", "چ", "پ", "ج", "ش" };
            DateTimeFormat.ShortestDayNames = new string[] { "ی", "د", "س", "چ", "پ", "ج", "ش" };
            DateTimeFormat.DayNames = new string[] { "یکشنبه", "دوشنبه", "ﺳﻪشنبه", "چهارشنبه", "پنجشنبه", "جمعه", "شنبه" };

            DateTimeFormat.AMDesignator = "ق.ظ";
            DateTimeFormat.PMDesignator = "ب.ظ";

            DateTimeFormat.ShortDatePattern = "yyyy-MM-dd";
            DateTimeFormat.LongDatePattern = "yyyy-MM-dd";

            DateTimeFormat.SetAllDateTimePatterns(new[] { "yyyy-MM-dd" }, 'd');
            DateTimeFormat.SetAllDateTimePatterns(new[] { "dddd, dd MMMM yyyy" }, 'D');
            DateTimeFormat.SetAllDateTimePatterns(new[] { "yyyy MMMM" }, 'y');
            DateTimeFormat.SetAllDateTimePatterns(new[] { "yyyy MMMM" }, 'Y');


        }

        public override Calendar Calendar
        {
            get { return cal; }
        }

        public override Calendar[] OptionalCalendars
        {
            get { return optionals; }
        }
        //convert shamsi to miladi
        public DateTime? ShamsiToMiladi(string _date)
        {
            try
            {
                if (_date != null && _date.Length == 10)
                {
                    string y = _date.Substring(0, 4);
                    string m = _date.Substring(5, 2);
                    if (m.Length < 2)
                    {
                        m = "0" + m;
                    }
                    string d = _date.Substring(8, 2);
                    if (d.Length < 2)
                    {
                        d = "0" + d;
                    }
                    int year = Convert.ToInt32(y);
                    int month = Convert.ToInt32(m);
                    int day = Convert.ToInt32(d);
                    PersianCalendar p = new PersianCalendar();
                    DateTime date = p.ToDateTime(year, month, day, 0, 0, 0, 0);

                    return Convert.ToDateTime(date.ToString("yyyy dd, MMMM"));
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        // convert milady to shamsi
        public string MiladiToShamsi(DateTime _date)
        {


            System.Globalization.PersianCalendar shamsi = new System.Globalization.PersianCalendar();
            DateTime sh;
            string strdate = null;
            strdate = _date.ToString("yyyy/MM/dd");
            sh = DateTime.Parse(strdate);
            int ysh = shamsi.GetYear(sh);
            int msh = shamsi.GetMonth(sh);
            int dsh = shamsi.GetDayOfMonth(sh);
            string d = dsh.ToString();
            string m = msh.ToString();
            if (d.Length < 2)
            {
                d = "0" + d;
            }
            if (m.Length < 2)
            {
                m = "0" + m;
            }
            return ysh.ToString() + "/" + m + "/" + d;
            // end convert datetime 
        }
        public string DateOfWeak(DateTime date)
        {
            var newWeekDays = new string[] { "یکشنبه", "دوشنبه", "سه شنبه", "چهارشنبه", "پنجشنبه", "جمعه", "شنبه" };
            var dtInfo = new DateTimeFormatInfo();
            dtInfo.AbbreviatedDayNames = newWeekDays;

            return date.ToString("ddd", dtInfo);
        }
        public string MonthOfWeak(DateTime date)
        {
            var MonthOfWeaks = new string[] { "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور", "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند", "" };
            var dtInfo = new DateTimeFormatInfo();
            dtInfo.AbbreviatedMonthNames = MonthOfWeaks;

            return date.ToString("ddd", dtInfo);
        }
        public string ShamsiDateFormat(DateTime date)
        {
            System.Globalization.PersianCalendar shamsi = new System.Globalization.PersianCalendar();
            DateTime sh;
            string strdate = null;
            strdate = date.ToString("yyyy/MM/dd");
            sh = DateTime.Parse(strdate);
            int ysh = shamsi.GetYear(sh);
            int msh = shamsi.GetMonth(sh);
            int dsh = shamsi.GetDayOfMonth(sh);
            string d = dsh.ToString();
            string m = msh.ToString();
            if (d.Length < 2)
            {
                d = "0" + d;
            }
            if (m.Length < 2)
            {
                m = "0" + m;
            }

            return DateOfWeak(date) + " " + d + " " + MonthOfWeak(date);
        }

        public string ToPersionDate(string datetime)
        {
            var miladidate = Convert.ToDateTime(datetime);
            var persianCalendar = new PersianCalendar();
            return String.Format("{0}/{1}/{2}",
                persianCalendar.GetYear(miladidate), persianCalendar.GetMonth(miladidate), persianCalendar.GetDayOfMonth(miladidate));
        }


    }

}

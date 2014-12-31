using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StockiiPanel
{
    class TimeControl
    {
        /// <summary> 
        /// 得到一个月的第一天 
        /// </summary> 
        /// <param name="someday">这个月的随便一天</param> 
        /// <returns>DateTime</returns> 
        public static DateTime GetFirstDayOfMonth(DateTime someday)
        {
            int totalDays = DateTime.DaysInMonth(someday.Year, someday.Month);
            DateTime result;
            int ts = 1 - someday.Day;
            result = someday.AddDays(ts);
            return result;
        }
        /// <summary> 
        /// 得到一个月的最后一天 
        /// </summary> 
        /// <param name="someday">这个月的随便一天</param> 
        /// <returns>DateTime</returns> 
        public static DateTime GetLastDayOfMonth(DateTime someday)
        {
            int totalDays = DateTime.DaysInMonth(someday.Year, someday.Month);
            DateTime result;
            int ts = totalDays - someday.Day;
            result = someday.AddDays(ts);
            return result;
        }

        /// <summary> 
        /// 得到本月的第一天日期 
        /// </summary> 
        /// <returns>DateTime</returns> 
        public static DateTime GetFirstDayOfMonth()
        {
            return GetFirstDayOfMonth(DateTime.Now);
        }
        /// <summary> 
        /// 得到本月的最有一天的日期 
        /// </summary> 
        /// <returns>DateTime</returns> 
        public static DateTime GetLastDayOfMonth()
        {
            return GetLastDayOfMonth(DateTime.Now);
        }

    }
}

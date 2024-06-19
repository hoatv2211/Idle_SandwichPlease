using System;
using System.Collections.Generic;
using UnityEngine;

namespace ThePattern.Extensions
{
    public static class DateTimeExtensions
    {
        public static long ToUnixTimestamp(this DateTime date)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0);
            return (long)(date - dateTime).TotalSeconds;
        }

        public static long ToUnixTimestamp(this DateTime? date) => date.HasValue ? date.Value.ToUnixTimestamp() : 0L;

        public static DateTime TimestampToDateTimeUTC(this long totalSecond)
        {
            DateTime dateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeUtc = dateTimeUtc.AddSeconds((double)totalSecond);
            return dateTimeUtc;
        }
    }
}
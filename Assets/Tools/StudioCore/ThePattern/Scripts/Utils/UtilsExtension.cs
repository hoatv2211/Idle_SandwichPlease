using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThePattern.Extensions
{
    public static class UtilsExtension
    {
        public static string FormatNumber(this long num, long conditionLargeThan = 100000)
        {
            if (num <= conditionLargeThan)
                return num.ToString("#,0");
            long num1 = (long)Math.Pow(10.0, (double)(int)Math.Max(0.0, Math.Log10((double)num) - 2.0));
            if (num1 > 0L)
                num = num / num1 * num1;
            if (num >= 10000000000L)
                return ((double)num / 1000000000.0).ToString("0.#") + "B";
            if (num >= 1000000000L)
                return ((double)num / 1000000000.0).ToString("0.##") + "B";
            if (num >= 100000000L)
                return ((double)num / 1000000.0).ToString("0.#") + "M";
            if (num >= 1000000L)
                return ((double)num / 1000000.0).ToString("0.##") + "M";
            if (num >= 100000L)
                return ((double)num / 1000.0).ToString("0.#") + "K";
            return num >= 10000L ? ((double)num / 1000.0).ToString("0.##k") + "K" : num.ToString("#,0");
        }
        public static string FormatNumber(this int num, long conditionLargeThan = 100000) => ((long)num).FormatNumber(conditionLargeThan);
        public static string FormatNumber(this float num, long conditionLargeThan = 100000) => ((long)Math.Round((double)num)).FormatNumber(conditionLargeThan);
    }
}
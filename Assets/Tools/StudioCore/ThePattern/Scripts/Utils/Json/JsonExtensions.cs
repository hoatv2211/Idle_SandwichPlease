using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace ThePattern.Extensions
{
    public static class JsonExtensions
    {
        public static string ToJson(this object data)
        {
            string jsonData = JsonConvert.SerializeObject(data);
            return jsonData;
        }

        public static string ToJsonFormat(this object data)
        {
            string jsonData = JsonConvert.SerializeObject(data, Formatting.Indented);
            return jsonData;
        }

        public static T ToObject<T>(this string jsonData)
        {
            T data = JsonConvert.DeserializeObject<T>(jsonData);
            return data;
        }

        private static string FormatJson(string str)
        {
            int count = 0;
            bool flag1 = false;
            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < str.Length; ++index)
            {
                char ch = str[index];
                switch (ch)
                {
                    case '"':
                        sb.Append(ch);
                        bool flag2 = false;
                        int num = index;
                        while (num > 0 && str[--num] == '\\')
                            flag2 = !flag2;
                        if (!flag2)
                        {
                            flag1 = !flag1;
                            break;
                        }
                        break;
                    case ',':
                        sb.Append(ch);
                        if (!flag1)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, count).ForEach<int>(item => sb.Append("    "));
                            break;
                        }
                        break;
                    case ':':
                        sb.Append(ch);
                        if (!flag1)
                        {
                            sb.Append(" ");
                            break;
                        }
                        break;
                    case '[':
                    case '{':
                        sb.Append(ch);
                        if (!flag1)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, ++count).ForEach<int>(item => sb.Append("    "));
                            break;
                        }
                        break;
                    case ']':
                    case '}':
                        if (!flag1)
                        {
                            sb.AppendLine();
                            Enumerable.Range(0, --count).ForEach<int>(item => sb.Append("    "));
                        }
                        sb.Append(ch);
                        break;
                    default:
                        sb.Append(ch);
                        break;
                }
            }
            return sb.ToString();
        }

        public static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (T obj in ie)
                action(obj);
        }
    }
}
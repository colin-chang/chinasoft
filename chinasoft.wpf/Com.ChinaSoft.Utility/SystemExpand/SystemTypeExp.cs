using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace System
{
    public static class SystemTypeExp
    {
        public static double ToNonNaN(this double d) => double.IsNaN(d) ? 0 : d;

        /// <summary>
        /// 转换为标准URL
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToUrl(this string str) => str.Replace('\\', '/');

        /// <summary>
        /// 转换为Url请求参数(进行两次UrlEncode)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToUrlParameter(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;
            return HttpUtility.UrlEncode(HttpUtility.UrlEncode(str));
        }

        /// <summary>
        /// 将文件后缀修改为xps
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string ToXpsExtension(this string fileName) => Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName) + ".xps");

        /// <summary>
        /// 包装字符串
        /// </summary>
        /// <param name="str"></param>
        /// <param name="mark"></param>
        /// <returns></returns>
        public static string Wrap(this string str, string mark) => mark + str + mark;

        /// <summary>
        /// 格式化标准Json字符串
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string ToJsonFormat(this string json)
        {
            var matches = Regex.Matches(json, "(\"?\\w+\"?):((\\[(\"?\\w+\"?\\,?)+\\])|(\"?(\\w\\.?)+\"?))");
            if (matches.Count <= 0)
                return null;
            StringBuilder sb = new StringBuilder("{");
            foreach (Match match in matches)
            {
                var kv = match.Value.Split(':');
                sb.Append(Warp(kv[0]));
                sb.Append(":");
                sb.Append(Warp(kv[1]));
                sb.Append(",");
            }
            sb.Remove(sb.Length - 1, 1).Append("}");
            return sb.ToString();
        }

        static string Warp(string str)
        {
            if (string.Equals(str, "null"))
                return str;

            if (str.StartsWith("[") && str.EndsWith("]"))
            {
                var arr = str.TrimStart('[').TrimEnd(']').Split(',');
                string[] result = new string[arr.Length];
                for (int i = 0; i < arr.Length; i++)
                {
                    string item = arr[i];
                    result[i] = item.StartsWith("\"") && item.EndsWith("\"") ? item : "\"" + item + "\"";
                }
                return "[" + string.Join(",", result) + "]";
            }

            return str.StartsWith("\"") && str.EndsWith("\"") ? str : "\"" + str + "\"";
        }
    }
}

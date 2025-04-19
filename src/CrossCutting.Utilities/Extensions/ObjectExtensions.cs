using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace Common.Utilities.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToQueryString(this object obj)
        {
            var nvc = new NameValueCollection();

            foreach (var prop in obj.GetType().GetProperties())
            {
                if (prop.GetValue(obj, null) is ICollection items)
                {
                    foreach (var listitem in prop.GetValue(obj) as IEnumerable)
                    {
                        nvc.Add(prop.Name, listitem != null ? listitem.ToString() : null);
                    }
                }
                else
                {
                    var value = prop.GetValue(obj);

                    if (value is DateTime)
                        nvc.Add(prop.Name, value != null ? ((DateTime)value).ToString("yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'") : null);
                    else
                        nvc.Add(prop.Name, value != null ? value.ToString() : null);
                }
            }

            StringBuilder sb = new StringBuilder();

            foreach (string key in nvc.Keys)
            {
                if (string.IsNullOrWhiteSpace(key)) continue;

                string[] values = nvc.GetValues(key);
                if (values == null) continue;

                foreach (string value in values)
                {
                    sb.Append(sb.Length == 0 ? "?" : "&");
                    sb.AppendFormat("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value));
                }
            }

            return sb.ToString();
        }
    }
}

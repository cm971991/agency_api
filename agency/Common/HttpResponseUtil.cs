using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace agency.Common
{
    /// <summary>
    /// 自动将对象，字符串装换为HttpResponseMessage
    /// </summary>
    public static class HttpResponseUtil
    {
        public static HttpResponseMessage AutoJson(this object obj, HttpStatusCode code = HttpStatusCode.OK, string DateTimeFormat = "yyyy-MM-dd HH:mm:ss")
        {
            string str;
            string mediaType;
            if (obj is string || obj is char)
            {
                str = obj.ToString();
                mediaType = "text/plain";
            }
            else
            {
                IsoDateTimeConverter timeFormat = new IsoDateTimeConverter { DateTimeFormat = DateTimeFormat };
                str = JsonConvert.SerializeObject(obj, Formatting.Indented, timeFormat);
                mediaType = "application/json";
            }
            var result = new HttpResponseMessage(code) { Content = new StringContent(str, Encoding.UTF8, mediaType) };
            return result;
        }
    }
}
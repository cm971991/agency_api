using agency.Common;
using log4net;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace agency.Api
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AgencyController : ApiController
    {
        private static ILog Log = LogManager.GetLogger(typeof(AgencyController));

        [HttpGet]
        public HttpResponseMessage Get(string url, string query = null, string param = null, string headKeys = null)
        {
            var u = GetUrl(url, param, query);

            List<KeyValuePair<string, string>> headers = null;
            if (headKeys != null)
            {
                var heads = HttpContext.Current.Request.Headers;
                var headKeysList = headKeys.Split(',');
                headers = heads.AllKeys.Where(x => headKeysList.Contains(x)).Select(key => new KeyValuePair<string, string>(key, heads[key])).ToList();
            }

            var result = HttpWebResponseUtility.Get(u, null, headers);

            return result.Item2.AutoJson(result.Item1);
        }

        [HttpPost]
        public HttpResponseMessage Post(string url, string param = null, string headKeys = null)
        {
            List<KeyValuePair<string, string>> headers = null;

            if (headKeys != null)
            {
                var heads = HttpContext.Current.Request.Headers;
                var headKeysList = headKeys.Split(',');
                headers = heads.AllKeys.Where(x => headKeysList.Contains(x)).Select(key => new KeyValuePair<string, string>(key, heads[key])).ToList();
            }

            if (param != null)
            {
                url += '/' + param;
            }

            Log.Info("url:" + url);

            string body = Request.Content.ReadAsStringAsync().Result;

            Log.Info("body:" + body);

            try
            {
                List<KeyValuePair<string, string>> list = SerializeString(body);
                var result = HttpWebResponseUtility.PostBody(url, list, null, headers);
                return result.Item2.AutoJson(result.Item1);
            }
            catch (Exception ex)
            {
                Log.Error("Post :" + ex.ToString());
                return new HttpResponseMessage();
            }
        }

        private string GetUrl(string url, string param, string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return (string.IsNullOrEmpty(param)) ? url : url + param;
            }
            return url + "?" + Uri.EscapeUriString(query);
        }

        private List<KeyValuePair<string, string>> SerializeString(string param)
        {
            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();

            param = Regex.Replace(param, @"\n|{|}| |""", "");

            if (!string.IsNullOrWhiteSpace(param))
            {
                string[] paramSplit = param.Split(',');

                for (int i = 0; i < paramSplit.Length; i++)
                {
                    string[] keyValue = paramSplit[i].Split(':');

                    list.Add(new KeyValuePair<string, string>(keyValue[0], keyValue[1]));
                }
            }

            return list;
        }

    }
}
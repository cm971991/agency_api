//---------------------------------------------------------------- 
//Copyright (C) 2011-2012 Nanjing Axon Technology Co.,Ltd
//Http://www.axon.com.cn 
// All rights reserved.
//<author>万刚</author>
//<createDate>2014/12/8 10:18:56</createDate>
//<description>Http.cs
//</description>
//----------------------------------------------------------------

using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace agency.Common
{
    /// <summary>
    /// 有关HTTP请求的辅助类
    /// </summary>
    public class HttpWebResponseUtility
    {
        private static ILog Log = LogManager.GetLogger(typeof(HttpWebResponseUtility));

        /// <summary>
        /// 创建GET方式的HTTP请求
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="encoding">编码，不填默认utf8</param>
        /// <param name="headers">请求头</param>
        public static Tuple<HttpStatusCode, string> Get(string url, Encoding encoding = null, IEnumerable<KeyValuePair<string, string>> headers = null)
        {
            string practicalResultJson;
            HttpStatusCode practicalHttpStatusCode;
            CreateHttpResponse(url, out practicalResultJson, out practicalHttpStatusCode, false, false, null, null, headers, encoding);
            return new Tuple<HttpStatusCode, string>(practicalHttpStatusCode, practicalResultJson);
        }

        /// <summary>
        /// 创建POST方式的HTTP请求(body形式)
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="body">请求主体</param>
        /// <param name="encoding">编码，不填默认utf8</param>
        /// <param name="headers">请求头</param>
        public static Tuple<HttpStatusCode, string> PostBody(string url, IEnumerable<KeyValuePair<string, string>> body, Encoding encoding = null, IEnumerable<KeyValuePair<string, string>> headers = null)
        {
            string practicalResultJson;
            HttpStatusCode practicalHttpStatusCode;
            CreateHttpResponse(url, out practicalResultJson, out practicalHttpStatusCode, true, false, body, null, headers, encoding);
            return new Tuple<HttpStatusCode, string>(practicalHttpStatusCode, practicalResultJson);
        }

        /// <summary>
        /// 创建POST方式的HTTP请求(json形式)
        /// </summary>
        /// <param name="url">请求的URL</param>
        /// <param name="json">请求主体</param>
        /// <param name="encoding">编码，不填默认utf8</param>
        /// <param name="headers">请求头</param>
        public static Tuple<HttpStatusCode, string> PostJson(string url, string json, Encoding encoding = null, IEnumerable<KeyValuePair<string, string>> headers = null)
        {
            string practicalResultJson;
            HttpStatusCode practicalHttpStatusCode;
            CreateHttpResponse(url, out practicalResultJson, out practicalHttpStatusCode, true, true, null, json, headers, encoding);
            return new Tuple<HttpStatusCode, string>(practicalHttpStatusCode, practicalResultJson);
        }

        private static void CreateHttpResponse(
            string url, out string practicalResultJson, out HttpStatusCode practicalHttpStatusCode, bool isPost, bool isJson, IEnumerable<KeyValuePair<string, string>> body, string json,
            IEnumerable<KeyValuePair<string, string>> headers, Encoding encoding)
        {
            ServicePointManager.ServerCertificateValidationCallback = (sndr, certificate, chain, sslPolicyErrors) => true;

            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            if (url.StartsWith("https"))
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            }

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = isPost ? "POST" : "GET";
            if (headers != null)
            {
                foreach (var headerKeyValue in headers)
                {
                    string key = headerKeyValue.Key;
                    string value = headerKeyValue.Value;
                    if (key.Equals("User-Agent"))
                    {
                        request.UserAgent = value;
                    }
                    else
                    {
                        request.Headers.Add(key.Trim(), value.Trim());
                    }
                }
            }

            if (isPost)
            {
                // json
                if (isJson)
                {
                    request.ContentType = "application/json";
                    if (!string.IsNullOrEmpty(json))
                    {
                        byte[] jsonContent = encoding.GetBytes(json);
                        request.ContentLength = jsonContent.Length;
                        Stream reqStream = request.GetRequestStream();
                        reqStream.Write(jsonContent, 0, jsonContent.Length);
                    }
                    else
                    {
                        request.ContentLength = 0;
                    }
                }
                // body
                else
                {
                    string paraUrlCoded = string.Empty;

                    if (body != null)
                    {
                        int i = 0;
                        foreach (var bodyKeyValue in body)
                        {
                            string key = bodyKeyValue.Key;
                            string value = bodyKeyValue.Value;
                            if (i > 0)
                            {
                                paraUrlCoded = paraUrlCoded + "&";
                            }
                            paraUrlCoded = paraUrlCoded + HttpUtility.UrlEncode(key) + "=" + HttpUtility.UrlEncode(value);
                            i++;
                        }
                    }

                    Log.Info("paraUrlCoded:" + paraUrlCoded);

                    request.ContentType = "application/x-www-form-urlencoded";
                    byte[] payload = encoding.GetBytes(paraUrlCoded);
                    request.ContentLength = payload.Length;
                    Stream writer = request.GetRequestStream();
                    writer.Write(payload, 0, payload.Length);
                }
            }

            HttpWebResponse response;
            try
            {
                response = request.GetResponse() as HttpWebResponse;
            }
            catch (WebException webEx)
            {
                Log.Error("GetResponse()异常");
                response = webEx.Response as HttpWebResponse;
            }

            if (response != null)
            {
                Stream stream = response.GetResponseStream();
                if (stream != null)
                {
                    StreamReader reader = new StreamReader(stream, encoding);
                    practicalResultJson = reader.ReadToEnd();
                    practicalHttpStatusCode = response.StatusCode;
                    return;
                }
            }
            else
            {

                Log.Error("response==null");
            }
            practicalResultJson = null;
            practicalHttpStatusCode = HttpStatusCode.InternalServerError;
        }
    }
}
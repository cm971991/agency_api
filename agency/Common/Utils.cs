using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace agency.Common
{
    public class Utils
    {
        /// <summary>
        /// 取得网站的根目录的URL
        /// </summary>
        /// <returns></returns>
        public static string Root
        {
            get
            {
                string AppPath = "";
                HttpContext HttpCurrent = HttpContext.Current;
                HttpRequest Req;
                if (HttpCurrent != null)
                {
                    Req = HttpCurrent.Request;
                    string UrlAuthority = Req.Url.GetLeftPart(UriPartial.Authority);
                    if (Req.ApplicationPath == null || Req.ApplicationPath == "/")
                    {
                        AppPath = UrlAuthority;                //直接安装在 Web 站点
                    }
                    else
                    {
                        AppPath = UrlAuthority + Req.ApplicationPath;                //安装在虚拟子目录下
                    }
                }
                return AppPath;
            }
        }

        /// <summary>
        /// 手机格式 验证
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        public static bool IsPhone(string phoneNumber)
        {
            Regex phoneRegex = new Regex(@"(^(13\d|14[57]|15[^4,\D]|17[678]|18\d)\d{8}|170[059]\d{7})$", RegexOptions.Compiled | RegexOptions.Singleline);
            return phoneRegex.IsMatch(phoneNumber);
        }
    }
}
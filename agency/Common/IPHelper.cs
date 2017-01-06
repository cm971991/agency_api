using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace agency.Common
{
    public class IPHelper
    {

        #region 获得IP
        /// <summary> 
        /// 取得客户端真实IP。如果有代理则取第一个非内网地址 
        /// </summary> 
        /// <example>
        ///  调用示例如下:
        ///     <code>
        ///     string IPAddress = Axon.IP.GetIPAddress();
        ///     </code>
        /// </example>
        /// <returns>string类型。点分十进制IP。</returns>
        public static string GetIPAddress()
        {
            string result = String.Empty;
            if (null != result || result != String.Empty)
            {
                if (IsLocaIP(result))
                {
                    result = "";
                }
            }
            if (result == null || result == String.Empty)
            {
                result = GetIPFromDail("HTTP_X_FORWARDED_FOR");
            }
            if (result == null || result == String.Empty)
            {
                result = GetIPFromDail("X_FORWARDED_FOR");
            }
            if (result == null || result == String.Empty)
            {
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            if (result == null || result == String.Empty)
            {
                result = HttpContext.Current.Request.UserHostAddress;
            }
            return result;
        }

        /// <summary>
        /// 从代理头中获取IP地址
        /// </summary>
        /// <param name="httpheadername"></param>
        /// <returns></returns>
        private static string GetIPFromDail(string httpheadername)
        {
            string result = HttpContext.Current.Request.ServerVariables[httpheadername];
            return result;
        }
        /// <summary>
        /// 判断IP是否是内网IP
        /// </summary>
        /// <example>
        ///  调用示例如下:
        ///     <code>
        ///     bool isLocal = Axon.IP.IsLocaIP("10.10.132.1");
        ///     </code>
        /// </example>
        /// <param name="ipaddress">string类型。IP地址。</param>
        /// <returns>bool类型。true是内网IP，false不是内网IP。</returns>
        public static bool IsLocaIP(string ipaddress)
        {
            if (IsIPAddress(ipaddress)
                                    && ipaddress.Substring(0, 3) != "10."
                                    && ipaddress.Substring(0, 7) != "192.168"
                                    && ipaddress.Substring(0, 7) != "172.16.")
            {
                return false;    //不是内网的地址 
            }
            else
            {
                return true;
            }
        }
        public static bool IsIPAddress(string ipaddress)
        {
            if (ipaddress == null || ipaddress == string.Empty || ipaddress.Length < 7 || ipaddress.Length > 15) return false;

            string regformat = @"^/d{1,3}[/.]/d{1,3}[/.]/d{1,3}[/.]/d{1,3}$";

            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(ipaddress);
        }
        #endregion


    }
}
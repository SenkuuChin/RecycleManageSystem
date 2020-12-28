using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace RecycleSystem.Ulitity
{
    public static class GetUserDeviceInfo
    {
        public static string GetUserIp(this HttpContext context)
        {
            var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (string.IsNullOrEmpty(ip))
            {
                ip = context.Connection.RemoteIpAddress.ToString();
            }
            return ip;
        }
        public static string UserAgent(this HttpContext context)
        {
            if (context == null)
            {
                return string.Empty;
            }
            return context.Request.Headers[HeaderNames.UserAgent].ToString();
        }
        #region 获取IP地址

        /// <summary> 
        /// 获取IP地址
        /// </summary> 
        /// <returns></returns>

        public static string GetIPAddress(HttpContext httpContext)
        {
            string ipv4 = String.Empty;
            foreach (IPAddress IPA in Dns.GetHostAddresses(httpContext.Connection.RemoteIpAddress.ToString()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    ipv4 = IPA.ToString();
                    break;
                }
            }
            if (ipv4 != String.Empty)
            {
                return ipv4;
            }
            foreach (IPAddress IPA in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {
                    ipv4 = IPA.ToString();
                    break;
                }
            }
            return ipv4;
        }

        #endregion
        #region 获取浏览器版本号

        /// <summary> 
        /// 获取浏览器版本号 
        /// </summary> 
        /// <returns></returns> 
        //public static string GetBrowser(HttpContext httpContext)
        //{

        //    //HttpBrowserCapabilities bc = httpContext.Request.Headers["User-Agent"].ToString();
        //    //return bc.Browser + bc.Version;
        //}

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ColaApp.Core
{
    /// <summary>
    /// WebClient扩展类
    /// </summary>
    public class XWebClient : WebClient
    {
        /// <summary>
        /// 重载方法，避免了中文的乱码
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            return request;
        }
    }
}

public void Test()
{
    return;
}

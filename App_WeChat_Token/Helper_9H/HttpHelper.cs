using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace Helper_9H
{
    public class HttpHelper
    {
        public static string Get(string url)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.ContentType = "application/json";

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                return streamReader.ReadToEnd();
            }
            return null;
        }

        public static string Post(string url, string requestBody)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/json";

            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            }

            // 参数
            Stream reqestStream = request.GetRequestStream();
            byte[] requestBytes = System.Text.Encoding.UTF8.GetBytes(requestBody);
            reqestStream.Write(requestBytes, 0, requestBytes.Length);

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                return streamReader.ReadToEnd();
            }
            return null;
        }

        private static bool CheckValidationResult(object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}

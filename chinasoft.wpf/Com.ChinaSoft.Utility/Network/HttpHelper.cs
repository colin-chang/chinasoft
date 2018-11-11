using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;


namespace Com.ChinaSoft.Utility.Network
{
    /// <summary>
    ///HTTP协议模拟请求帮助类
    /// </summary>
    public class HttpHelper
    {
        #region Request Header
        private readonly string Accept = "*/*";
        private readonly string UserAgent = ConfigurationManager.AppSettings["UserAgent"];
        private readonly string ContentType = ConfigurationManager.AppSettings["ContentType"];
        #endregion

        #region Helper Method

        HttpWebRequest CreateWebRequest(string url, HttpMethod method)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method.ToString();
            request.UserAgent = UserAgent;
            request.ContentType = ContentType;
            request.Accept = Accept;
            request.Timeout = 1500;

            return request;
        }

        async Task<string> GetResponseAsync(HttpWebRequest request)
        {
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("utf-8")))
                        {
                            string str = await reader.ReadToEndAsync();
                            return str;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        bool HandleUrl(string originalUrl, bool absoluteUrl, out string url)
        {
            url = absoluteUrl ? originalUrl : ConfigurationManager.AppSettings["ApiUrl"];
            return !string.IsNullOrWhiteSpace(url);
        }

        bool HandleUrl(string originalUrl, bool absoluteUrl, bool isGetMethod, Dictionary<string, string> param, out string url)
        {
            if (!isGetMethod)
                return this.HandleUrl(originalUrl, absoluteUrl, out url);

            string baseUrl = absoluteUrl ? originalUrl : ConfigurationManager.AppSettings["ApiUrl"];
            url = param != null && param.Count() > 0 ? baseUrl + "?" + string.Join("&", param.Select(kv => string.Format("{0}={1}", kv.Key, kv.Value.ToUrlParameter()))) : baseUrl;
            return !string.IsNullOrWhiteSpace(url);
        }

        #endregion

        #region Get
        public async Task<string> GetAsync(string url, Dictionary<string, string> param, bool absoluteUrl)
        {
            if (!HandleUrl(url, absoluteUrl, true, param, out url)) return null;

            HttpWebRequest request = this.CreateWebRequest(url, HttpMethod.Get);

            return await this.GetResponseAsync(request);
        }

        public async Task<string> GetAsync(Dictionary<string, string> param) => await this.GetAsync(null, param, false);

        public async Task<string> GetFileAsync(string address)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileName = Path.Combine(path, Path.GetFileName(address));
            if (File.Exists(fileName))
                return fileName;

            try
            {
                using (WebClient wc = new WebClient())
                {
                    await wc.DownloadFileTaskAsync(address, fileName);
                    return fileName;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region POST

        public async Task<string> PostAsync(string url, string data, bool absoluteUrl)
        {
            if (!HandleUrl(url, absoluteUrl, out url)) return null;

            HttpWebRequest request = this.CreateWebRequest(url, HttpMethod.Post);
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            request.ContentLength = bytes.Length;
            using (Stream writeStream = request.GetRequestStream())
            {
                writeStream.Write(bytes, 0, bytes.Length);
            }

            return await this.GetResponseAsync(request);
        }

        public async Task<string> PostAsync(string data) => await this.PostAsync(null, data, false);

        #endregion

        #region Put

        public async Task<string> PutAsync(string url, string data, bool absoluteUrl)
        {
            if (!HandleUrl(url, absoluteUrl, out url)) return null;

            HttpWebRequest request = this.CreateWebRequest(url, HttpMethod.Put);
            byte[] bytes = data == null ? new byte[0] : Encoding.UTF8.GetBytes(data);
            request.ContentLength = bytes.Length;
            using (Stream writeStream = request.GetRequestStream())
            {
                writeStream.Write(bytes, 0, bytes.Length);
            }

            return await this.GetResponseAsync(request);
        }

        //public async Task<string> PutAsync(string url, string id, string data, bool absoluteUrl)
        //{
        //    if (!HandleUrl(url, absoluteUrl, out url)) return null;

        //    HttpWebRequest request = this.CreateWebRequest(url, HttpMethod.Put);
        //    byte[] bytes = data == null ? new byte[0] : Encoding.UTF8.GetBytes(data);
        //    request.ContentLength = bytes.Length;
        //    using (Stream writeStream = request.GetRequestStream())
        //    {
        //        writeStream.Write(bytes, 0, bytes.Length);
        //    }

        //    return await this.GetResponseAsync(request);
        //}

        public async Task<string> PutAsync(string url)
        {
            return await this.PutAsync(url, null, true);
        }

        public async Task<string> PutAsync(string url, string data)
        {
            return await this.PutAsync(url, data, true);
        }

        #endregion

        #region Delete
        public async Task<string> DeleteAsync(string id) => await this.DeleteAsync(null, id, false);

        public async Task<string> DeleteAsync(string url, string id, bool absoluteUrl)
        {
            if (!this.HandleUrl(url, absoluteUrl, out url)) return null;

            HttpWebRequest request = this.CreateWebRequest(url, HttpMethod.Delete);

            return await this.GetResponseAsync(request);
        }
        #endregion
    }
}

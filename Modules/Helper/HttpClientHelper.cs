using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using vietjet_series_booking_dotnet.App.Controllers;

namespace vietjet_series_booking_dotnet.Modules.Helper
{
    public class HttpClientHelper : BaseController
    {
        public static HttpClient HttpClientVietjet = new HttpClient() { Timeout = TimeSpan.FromSeconds(30) };
        public static HttpClient HttpClientVietjetair = new HttpClient() { Timeout = TimeSpan.FromSeconds(30) };
        public static HttpClient HttpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(30) };
        
        public static async Task<(string,int?)> SendRequestAsync(string url,HttpMethod method, IDictionary<string,object> header,object data = null)
        {
            
            var requestMessage = new HttpRequestMessage(method, url);
            if(header != null)
            {
                foreach(var pa in header)
                {
                    requestMessage.Headers.Add(pa.Key, pa.Value.ToString());
                }
            }
            HttpContent content = null;
            if (data != null)
            {
                content = new StringContent((string)data, Encoding.UTF8, "application/json");
            }
            requestMessage.Content = content;
            using(var rq = await HttpClient.SendAsync(requestMessage).ConfigureAwait(false))
            {
                if (rq.IsSuccessStatusCode)
                {
                    if (rq.Content != null)
                    {
                        return (await rq.Content.ReadAsStringAsync().ConfigureAwait(false),(int)rq.StatusCode);
                    }
                    else
                        return (null,null);
                }
                else
                {
                   if(rq.Content != null)
                    {
                        return (await rq.Content.ReadAsStringAsync().ConfigureAwait(false),null);
                    }
                    throw new Exception($"request to {url} error ! StatusCode = {rq.StatusCode}");
                }
            }
        }
        public static async Task<(string,int?)> PutCallAPI(string url, object jsonObject)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
                    var response = await client.PutAsync(url, content);
                    if (response != null)
                    {
                        var jsonString = (await response.Content.ReadAsStringAsync(),(int)response.StatusCode);
                        return jsonString;
                    }
                }
            }
            catch (Exception ex)
            {
                return (ex.Message, null);
            }
            return (null,null);
        }
        //public static async Task<T> SendVietjetRequestAsync<T>(string url,HttpMethod method,IDictionary<string,object> header)
        //{
        //    var content = await SendRequestAsync(url, method, header);
        //    if (!string.IsNullOrEmpty(content))
        //        return JsonConvert.DeserializeObject<T>(content);
        //    else
        //        return default(T);
        //}

        //public static async Task<string> SendVietjetRequestAsync(string url, HttpMethod method, IDictionary<string, object> header=null)
        //{
        //    var content = await SendRequestAsync(url, method, header);
        //    return content.ToString();
        //}
    }
}

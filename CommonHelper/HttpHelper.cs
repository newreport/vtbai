using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    public static class HttpHelper
    {

        public static JObject HttpGet(string url, Dictionary<string, string> param = null)
        {
            string fullUrl = url;
            if (param != null && param.Count > 0)
            {
                fullUrl += $"?{param.FirstOrDefault().Key}={param.FirstOrDefault().Value}";
                param.Remove(param.FirstOrDefault().Key);
                foreach (var item in param)
                {
                    fullUrl += $"&{item.Key}={item.Value}";
                }
            }
            string responseBody = @"{}";
            try
            {
                using var task = new HttpClient().GetAsync(fullUrl);
                responseBody = task.Result.Content.ReadAsStringAsync().Result;
            }
            catch (Exception e)
            {
                Log.WriteLine("\nException Caught!");
                Log.WriteLine($"Message :{e.Message}");
            }
            return JObject.Parse(responseBody);
        }

        public static async Task<JObject> HttpGetAsync(string url, Dictionary<string, string> param=null)
        {
            string fullUrl = url;
            if (param != null && param.Count > 0)
            {
                fullUrl += $"?{param.FirstOrDefault().Key}={param.FirstOrDefault().Value}";
                param.Remove(param.FirstOrDefault().Key);
                foreach (var item in param)
                {
                    fullUrl += $"&{item.Key}={item.Value}";
                }
            }
            string responseBody = @"{}";
            try
            {
                using HttpResponseMessage repose = await new HttpClient().GetAsync(fullUrl);
                repose.EnsureSuccessStatusCode();
                responseBody = await repose.Content.ReadAsStringAsync();

            }
            catch (Exception e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return JObject.Parse(responseBody);
        }

        public static async Task<JObject> HttPostAsync(string url, Dictionary<string, string> bodyData)
        {
            return await Task.Run(() => { return new JObject(); });
        }

    }
}

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonHelper
{
    public class HttpHelper
    {

        public static async Task<JObject> HttpGet(string url, List<string> param)
        {
            string fullUrl = url;
            if (param.Count > 0)
            {
                fullUrl += $"?{param.FirstOrDefault()}";
                param.RemoveAt(0);
                foreach (var item in param)
                {
                    fullUrl += "&" + item;
                }
            }
            string responseBody= @"{}";
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

    }
}

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HeShuiLa
{
    internal class DouBaoAI
    {
        private const string ENDPOINT_ID = "";
        private const string API_KEY = "";
        HttpWebRequest hwr = WebRequest.CreateHttp("https://ark.cn-beijing.volces.com/api/v3/chat/completions");
        public async Task<string> Request(string prompt)
        {
            hwr.Method = "POST";
            hwr.ContentType = "application/json";
            hwr.Headers.Add("Authorization", $"Bearer {API_KEY}");
            var msg = JObject.FromObject(new { model = ENDPOINT_ID, messages = new[] { new { role = "user", content = prompt } } }).ToString(Newtonsoft.Json.Formatting.None);
            hwr.ContentLength = Encoding.UTF8.GetBytes(msg).Length;
            var stream = await hwr.GetRequestStreamAsync();
            using (StreamWriter sr = new StreamWriter(stream))
            {
                await sr.WriteAsync(msg);
                await sr.FlushAsync();
            }
            var resp = await hwr.GetResponseAsync();
            using (var sr = new StreamReader(resp.GetResponseStream()))
            {
                var respJson = await JObject.LoadAsync(new JsonTextReader(sr));
                var content = respJson["choices"].First["message"]["content"].ToString();
                return content;
            }
        }
    }
}

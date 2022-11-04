using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace testWebApi
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string url = $"http://10.20.16.153:2234/";
            string signInJsonString = readJsonFile(System.Environment.CurrentDirectory + @"\json2.json");
            string sendMessageJsonString = readJsonFile(System.Environment.CurrentDirectory + @"\json1.json");

            string response = "";
            response = await GetToken(url,signInJsonString);
            Console.WriteLine(response);

            JWT_TokenResponse tokenResponse = JsonConvert.DeserializeObject<JWT_TokenResponse>(response);

            //response = await SendLineNotifyMassage(url,tokenResponse.token, sendMessageJsonString);

            response = await GetRequestRecord(url, tokenResponse.token);

            Console.WriteLine(response);

            Console.ReadKey();
        }
        async static Task<string> SendLineNotifyMassage(string url,string token, string data)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            HttpContent contentPost = new StringContent(data, Encoding.UTF8, "application/json");

            HttpResponseMessage result = await httpClient.PostAsync(url + $"LineNotify/SendMessage/NoProxy", contentPost);

            string res = await result.Content.ReadAsStringAsync();

            return res;
        }
        async static Task<string> GetToken(string url,string Data)
        {
            var httpClient = new HttpClient();

            HttpContent contentPost = new StringContent(Data, Encoding.UTF8, "application/json");

            HttpResponseMessage result = await httpClient.PostAsync(url + "signin", contentPost);

            string res = await result.Content.ReadAsStringAsync();

            return res;
        }
        async static Task<string> GetRequestRecord(string url, string token)
        {
            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage result = await httpClient.GetAsync(url + "getallrequestrecord");

            string res = await result.Content.ReadAsStringAsync();

            return res;
        }
        private static string readJsonFile(string filePath)
        {
            StreamReader r = new StreamReader(filePath, Encoding.UTF8);
            string jsonString = r.ReadToEnd();
            r.Dispose();
            return jsonString;
        }
    }
    class JWT_TokenResponse
    {
        public string token { get; set; }
    }
}

using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace scale_lite
{
    class apilayer
    {
        public string ObtaingGet(string sUrl, string sHead, string sParameters, string sToken)
        {
            string sValues = "";

            string sComplete = string.Concat(sUrl, "/", sHead);

            var client = new RestClient(sComplete);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Authorization", sToken);
            request.AlwaysMultipartFormData = true;
            IRestResponse response = client.Execute(request);
            sValues = response.Content;

            return sValues;
        }
    }

    //private static void PostItem(string data)
    //{
    //    var url = $"http://localhost:8080/items";
    //    var request = (HttpWebRequest)WebRequest.Create(url);
    //    string json = $"{{\"data\":\"{data}\"}}";
    //    request.Method = "POST";
    //    request.ContentType = "application/json";
    //    request.Accept = "application/json";
    //    using (var streamWriter = new StreamWriter(request.GetRequestStream()))
    //    {
    //        streamWriter.Write(json);
    //        streamWriter.Flush();
    //        streamWriter.Close();
    //    }
    //    try
    //    {
    //        using (WebResponse response = request.GetResponse())
    //        {
    //            using (Stream strReader = response.GetResponseStream())
    //            {
    //                if (strReader == null) return;
    //                using (StreamReader objReader = new StreamReader(strReader))
    //                {
    //                    string responseBody = objReader.ReadToEnd();
    //                    // Do something with responseBody
    //                    Console.WriteLine(responseBody);
    //                }
    //            }
    //        }
    //    }
    //    catch (WebException ex)
    //    {
    //        // Handle error
    //    }
    //}


}

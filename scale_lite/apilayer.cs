using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
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
}

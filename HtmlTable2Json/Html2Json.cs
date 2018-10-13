using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HtmlTable2Json
{
    public static class Html2Json
    {
        [FunctionName("Html2Json")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string html = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "html", true) == 0)
                .Value;

            if (html == null)
            {
                // Get request body
                dynamic data = req.Content.ReadAsStringAsync().Result;
                html = data;
            }

            //ceate an Html Document object
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            //get the table tag content from Html document
            var htmltableBody = htmlDoc.DocumentNode;

            //generate a json object from html table
            object json = Getjson(htmltableBody);

            return req.CreateResponse(HttpStatusCode.OK, json);
        }

        private static object Getjson(HtmlNode htmlNode)
        {
            // create a json object to store the data from htmlNode
            var json = new JObject();

            //recurring each element in HtmlNode and create a key/Value pairs arrays
            foreach(var row in htmlNode.SelectNodes("//tr"))
            {
                // recurring the column in the table
                if (row.HasChildNodes)
                {
                    var recs = row.SelectNodes("//td");
                    // add json data from table td
                    json[recs[0].InnerText.ToString()] = recs[1].InnerText.ToString();
                }
            }
            // creating json object
            return JsonConvert.SerializeObject(json);
        }
    }
}

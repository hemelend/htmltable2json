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
                // removing carring return and new line characters
                html = data.Replace("\r\n","");
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
            // getting the table tags from Specialist Finder Email
            var htmltable = htmlNode.SelectSingleNode("//table");
            // getting the elements in the html table tag
            HtmlNodeCollection childnodes = htmltable.ChildNodes;

            //recurring each element in HtmlNode and create a key/Value pairs arrays
            foreach (var node in childnodes)
            {
                if (node.NodeType == HtmlNodeType.Element)
                {
                    // getting the table with the Specialist Finder information
                    var tablerows = node.SelectNodes("//table[@class='details']/tbody[1]/tr");
                    // recurring the rows in the html table
                    foreach (var row in tablerows)
                    {
                        if (row.NodeType == HtmlNodeType.Element)
                        {
                            // recurring the column in the table
                            var recs = row.Descendants("td").ToArray();
                            json[recs[0].InnerText.ToString()] = recs[1].InnerText.ToString();
                        }
                    }
                }
            }
            // creating json object
            return JsonConvert.SerializeObject(json);
        }
    }
}

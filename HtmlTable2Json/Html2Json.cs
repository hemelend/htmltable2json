using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;


namespace HtmlTable2Json
{
    public static class Html2Json
    {
        [FunctionName("Html2Json")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequestMessage req, TraceWriter log)
        {
            log.Info("C# HTTP trigger function processed a request.");

            // parse query parameter
            string name = req.GetQueryNameValuePairs()
                .FirstOrDefault(q => string.Compare(q.Key, "name", true) == 0)
                .Value;

            if (name == null)
            {
                // Get request body
                dynamic data = req.Content.ReadAsStringAsync().Result;
                name = data;
            }

            var html = name;

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var htmltableBody = htmlDoc.DocumentNode.SelectSingleNode("//table");



            //return req.CreateResponse(HttpStatusCode.OK, htmlBody.OuterHtml);
        }
    }
}

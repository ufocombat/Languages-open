using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Languages.Models;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace Languages.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Languages()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Send(String Description)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("https://northeurope.api.cognitive.microsoft.com/text/analytics/v2.0/languages");
            httpWebRequest.Method = "POST";

            httpWebRequest.Headers.Add("Content-Type:application/json");
            httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key:61d917*****");

            var documents = new Documents();
            documents.Add(new Document(Description));

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(documents);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            String response;

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8))
            {
                response = streamReader.ReadToEnd();
            }

            var result = JsonConvert.DeserializeObject<DocumentsResult>(response);
            var doc = result.documents[0].detectedLanguages[0];

            /*Sentiment*/

            var httpWebRequest2 = (HttpWebRequest)WebRequest.Create("https://northeurope.api.cognitive.microsoft.com/text/analytics/v2.0/sentiment");
            httpWebRequest2.Method = "POST";

            httpWebRequest2.Headers.Add("Content-Type:application/json");
            httpWebRequest2.Headers.Add("Ocp-Apim-Subscription-Key:61d917d5bf68483eb5c803f087923d73");

            var sdoc = new Document(Description);
            sdoc.language = doc.iso6391Name;

            var documents2 = new Documents();
            documents2.Add(sdoc);

            using (var streamWriter = new StreamWriter(httpWebRequest2.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(documents2);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse2 = (HttpWebResponse)httpWebRequest2.GetResponse();
            String response2;

            using (var streamReader = new StreamReader(httpResponse2.GetResponseStream(), Encoding.UTF8))
            {
                response2 = streamReader.ReadToEnd();
            }

            var result2 = JsonConvert.DeserializeObject<DocumentsResult>(response2);
            var doc2 = result2.documents[0];

            /*Key Phrases*/

            var httpWebRequest3 = (HttpWebRequest)WebRequest.Create("https://northeurope.api.cognitive.microsoft.com/text/analytics/v2.0/keyPhrases");
            httpWebRequest3.Method = "POST";

            httpWebRequest3.Headers.Add("Content-Type:application/json");
            httpWebRequest3.Headers.Add("Ocp-Apim-Subscription-Key:61d917d5bf68483eb5c803f087923d73");

            using (var streamWriter = new StreamWriter(httpWebRequest3.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(documents2);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse3 = (HttpWebResponse)httpWebRequest3.GetResponse();
            String response3;

            using (var streamReader = new StreamReader(httpResponse3.GetResponseStream(), Encoding.UTF8))
            {
                response3 = streamReader.ReadToEnd();
            }

            var result3 = JsonConvert.DeserializeObject<DocumentsResult>(response3);
            var doc3 = result3.documents[0];

            try
            {
                SqlConnection connection = new SqlConnection("Server=tcp:***.database.windows.net,1433;Initial Catalog=FastReportSQL;Persist Security Info=False;User ID=***;Password=***;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
                connection.Open();
                SqlCommand cmd = new SqlCommand($"INSERT INTO reviews(Review,Language,LanguageCode,probability,sentiment) VALUES (@desc,@name,@lang,@prob,@sent)",connection);
                cmd.Parameters.AddWithValue("desc",Description);
                cmd.Parameters.AddWithValue("name",doc.name);
                cmd.Parameters.AddWithValue("lang",doc.iso6391Name);
                cmd.Parameters.AddWithValue("prob",doc.score);
                cmd.Parameters.AddWithValue("sent",doc2.score);

                cmd.ExecuteNonQuery();
                connection.Close();

                return new ObjectResult($"<p>Language: {doc.name} ({Math.Round(doc.score*100)}%)</p> <p>Sentiment: {Math.Round(doc2.score*100)}% </p> <p>{response3}</p>");
            }
            catch (Exception e)
            {
               return new ObjectResult(e.ToString());
               //return new ObjectResult($"Please, do it correctlly...");
            }
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

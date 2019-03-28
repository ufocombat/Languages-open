using System;
using Languages.Models;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace Languages.Extension
{
    public static class Documents_Request
    {
        public static void GetRequest(this Documents documents, String service, String text)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create($"https://northeurope.api.cognitive.microsoft.com/text/analytics/v2.0/{service}");
            httpWebRequest.Method = "POST";

            httpWebRequest.Headers.Add("Content-Type:application/json");
            httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key:61d917d5bf68483eb5c803f087923d73");

            documents.documents.Clear();
            documents.Add(new Document(text));

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
        }
    }
}

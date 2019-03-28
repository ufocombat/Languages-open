using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Languages.Models
{
    /*Request*/

    public class Document
    {
        public String id = "1";
        public String text = String.Empty;
        public String language;

        public Document(String text): base()
        {
            this.text = text;
        }
    }

    public class Documents
    {
        public List<Document> documents = new List<Document>();
        public void Add(Document document)
        {
            documents.Add(document);
        }
    }

    /*Response*/
    //{"documents":[{"id":"1","detectedLanguages":[{"name":"English","iso6391Name":"en","score":1.0}]}],"errors":[]}
    //{"documents":[{"id":"1","score":0.87192869186401367}],"errors":[]}
    //{"documents":[{"id":"1","keyPhrases":["Ленинском районе","проишествий","страна"]}],"errors":[]}

    public class DocumentResult
    {
        public String id;
        public decimal score;
        public List<DetectedLanguage> detectedLanguages;
    }

    public class DetectedLanguage
    {
        public String name;
        public String iso6391Name;
        public decimal score;
    }

    public class DocumentsResult
{
        public List<DocumentResult> documents = new List<DocumentResult>();
        public String[] errors;
    }
}

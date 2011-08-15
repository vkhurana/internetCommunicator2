using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PluginSDK;
using System.Web;
using System.Net;
using System.Xml;
using System.IO;

namespace YahooAnswersPlugin
{
    [StatelessChatPluginAttribute("uses the yahoo answers api to get answers :)")]
    class YahooAnswers : IStatelessChatInterface
    {

        private string GetYahooAnswer(string question)
        {
            string request = YahooAnswersBaseAddress + "appid=" + YahooAppID;
            request += "&";
            request += "query=";
            request += HttpUtility.UrlEncode(question);
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(request);
            HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
            XmlDocument xmldoc = new XmlDocument();
            StreamReader reader = new StreamReader(webResponse.GetResponseStream());
            string xmlResponse = reader.ReadToEnd();
            xmldoc.LoadXml(xmlResponse);
            XmlNamespaceManager xmlNameSpace = new XmlNamespaceManager(xmldoc.NameTable);
            xmlNameSpace.AddNamespace("ya", "urn:yahoo:answers");
            XmlNode selectedQuestion = xmldoc.SelectSingleNode("/ya:ResultSet/ya:Question/ya:Subject", xmlNameSpace);
            XmlNode selectedAnswer = xmldoc.SelectSingleNode("/ya:ResultSet/ya:Question/ya:ChosenAnswer", xmlNameSpace);
            //XmlNodeList yahooQuestions = xmldoc.SelectNodes("/ya:ResultSet/ya:Question/ya:Subject", xmlNameSpace);
            //XmlNodeList yahooAnswers = xmldoc.SelectNodes("/ya:ResultSet/ya:Question/ya:ChosenAnswer", xmlNameSpace);

            return "\nQuestion: " + selectedQuestion.InnerText + "\nAnswer: " + selectedAnswer.InnerText;
        }

        private string YahooAnswersBaseAddress = "http://answers.yahooapis.com/AnswersService/V1/questionSearch?type=resolved&";
        private string YahooAppID = "<yahoo api key>";

        public string GetCommandResult(string cmdArgs)
        {
            return GetYahooAnswer(cmdArgs);
        }
        public string GetCommandTrigger()
        {
            return "question";
        }
    }
}

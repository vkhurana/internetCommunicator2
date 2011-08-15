using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Globalization;
using System.Web;
using System.Web.Script.Serialization;

namespace InterComm.Helpers
{
    public class TranslateAPI
    {
        public class GoogleAjaxResponse<T>
        {
            public T responseData = default(T);
        }
        public class TranslationResponse
        {
            public string translatedText = string.Empty;
            public object responseDetails = null;
            public HttpStatusCode responseStatus = HttpStatusCode.OK;
        }
        public class Translator
        {
            private static JavaScriptSerializer serializer = new JavaScriptSerializer();
            public static string TranslateText(string inputText, string fromLanguage, string toLanguage)
            {
                string referrer = SessionSettings.googlesitereferrer;
                string key = SessionSettings.googleapikey;

                string requestUrl = string.Format(
                    "http://ajax.googleapis.com/ajax/services/language/translate?v=1.0&q={0}&langpair={1}|{2}&key={3}",
                    HttpUtility.UrlEncode(inputText),
                    fromLanguage.ToLowerInvariant(),
                    toLanguage.ToLowerInvariant(),
                    key
                    );
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(requestUrl);
                req.Referer = referrer;
                try
                {
                    HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                    string responseJson = new StreamReader(res.GetResponseStream()).ReadToEnd();
                    GoogleAjaxResponse<TranslationResponse> translation = serializer.Deserialize<GoogleAjaxResponse<TranslationResponse>>(responseJson);
                    if (translation != null && translation.responseData != null && translation.responseData.responseStatus == HttpStatusCode.OK)
                    {
                        return translation.responseData.translatedText;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                catch
                {
                    return string.Empty;
                }
            }
            public static string TranslateText(string inputText, CultureInfo fromLanguage, CultureInfo toLanguage)
            {
                return TranslateText(inputText, fromLanguage.TwoLetterISOLanguageName, toLanguage.TwoLetterISOLanguageName);
            }
        }
    }
}

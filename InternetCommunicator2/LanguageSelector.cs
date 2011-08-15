using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using InterComm.Helpers;
using System.Globalization;

namespace InterComm
{
    class LanguageSelector
    {
        public class LanguageInfo
        {
            public string shortname;
            public string fullname;
            public int langcode;
            public LanguageInfo(string shortName, string fullName, int langCode)
            {
                shortname = shortName;
                fullname = fullName;
                langcode = langCode;
            }
        }

        private List<LanguageInfo> languagesList = new List<LanguageInfo>();

        public LanguageSelector()
        {
            if (!File.Exists(SessionSettings.langaugesfile))
            {
                Logger.LogInformation("error loading languages file " + SessionSettings.langaugesfile);
                throw new FileNotFoundException("error opening languages file");
            }
            string[] languages = File.ReadAllLines(SessionSettings.langaugesfile);
            foreach (string language in languages)
            {
                string[] langInfo = language.Split(new char[] { ',' });
                if (langInfo.Length != 3)
                {
                    Logger.LogInformation("languages file is in incorrect format!");
                    throw new InvalidDataException("languages file is in incorrect format");
                }

                LanguageInfo lang = new LanguageInfo(langInfo[0], langInfo[2], int.Parse(langInfo[1].ToLower().Replace("0x", ""), NumberStyles.HexNumber));
                languagesList.Add(lang);
            }
            Logger.LogInformation("loaded " + languagesList.Count + " possible languages...");
        }
        public LanguageInfo GetLanguageInfo(string shortName)
        {
            return (from lang in languagesList where lang.shortname == shortName select lang).First();
        }
        public LanguageInfo GetRandomLanguage()
        {
            Random rnd = new Random(DateTime.Now.Second);
            int index = rnd.Next(0, languagesList.Count-1);
            LanguageInfo lang = languagesList.ElementAt(index);
            while (lang.shortname == SessionSettings.defaultlanguage)
            {
                index = rnd.Next(0, languagesList.Count-1);
                lang = languagesList.ElementAt(index);
            }
            return lang;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlSearcher
{
    public class WordConverter
    {
        public string WebsitePlate;
        public int WordsRequired;
        public string BetweenWords;

        public WordConverter(string websiteplate, int wordsrequired, string betweenwords)
        {
            WebsitePlate = websiteplate;
            WordsRequired = wordsrequired;
            BetweenWords = betweenwords;
        }
        public WordConverter()
        {

        }

        public string ConvertWords(List<string> Words)
        {
            string FullWebAddress = WebsitePlate;
            for (int i = 0; i < Words.Count; i++)
            {
                FullWebAddress = FullWebAddress.Replace("{" + (i + 1) + "}", Words[i]);
            }
            return FullWebAddress;
        }
    }
}

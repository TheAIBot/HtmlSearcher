using System;
using System.Collections.Generic;
using System.Threading;
using HtmlAgilityPack;

namespace HtmlSearcher
{
    /* To Do List:
     * Make Understandable Exceptions
     * Fix Orders
     * Allow it to understand when something consist of multiple patterns
     * Make it output a lot more data that can be used in the UI
     */
    public static class Search
    {
        public static event ProgressEventHandler OnProgressChanged;

        public static List<ScrapedInfo> FindText(ScrapInfo WebInfo)
        {
            DownloadedWebSites DSites = WebStuff.GetHtmlCode(WebInfo);
            AnalyzedDocument ADoc = Analyzer.AnalyseDocument(DSites.WebsiteToAnalyze, WebInfo);
            List<ScrapedInfo> SInfo = ProcessWebsite.FindTexts(WebInfo, DSites, ADoc);
            return SInfo;
        }

        internal static void SetUpdateEvent(string Text)
        {
            if (OnProgressChanged != null)
            {
                OnProgressChanged(null, new ProgressEventArgs(Text));
            }
        }
    }

    public delegate void ProgressEventHandler(object Source, ProgressEventArgs e);

    public class ProgressEventArgs : EventArgs
    {
        public string UpdateText;

        public ProgressEventArgs(string Text)
        {
            UpdateText = Text;
        }
    }

    public enum POptions
    {
        Auto,
        Manual
    }

    public enum TextAmount
    {
        Single,
        Multiple
    }


    public class ScrapInfo
    {
        internal bool UseWordConverter
        {
            get
            {
                return !(WConverter == null);
            }
        }
        public WordConverter WConverter;
        public List<GroupToFind> Groups;
        public string WebToAnalyze;
        public List<string> WebAddresses = new List<string>();
    }
    public class GroupToFind
    {
        public List<TextToFindInfo> TextsToFind = new List<TextToFindInfo>();
    }
    public class TextToFindInfo
    {
        public List<string> TextsToFind;
        public TextAmount TextAmountToFind;
        public string Name;

    }



    public class ScrapedInfo
    {
        public string WebSite;
        public HtmlDocument HDoc;
        public List<FoundGroup> Groups = new List<FoundGroup>();

    }
    public class FoundGroup
    {
        public List<FoundTextInfo> FoundTextInfos = new List<FoundTextInfo>();
    }
    public class FoundTextInfo
    {
        public List<string> FoundText = new List<string>();
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlSearcher
{
    public static class ProcessWebsite
    {
        internal static List<ScrapedInfo> FindTexts(ScrapInfo FInfo, DownloadedWebSites DSites, AnalyzedDocument ADoc)
        {
            foreach (ScrapedInfo SInfo in DSites.SInfos)
            {
                FoundGroup FGroup = new FoundGroup();
                foreach (AnalyzedElement AElement in ADoc.AElements)
                {
                    FoundTextInfo FText = GetTextFromHtmlDocument(SInfo.HDoc, AElement);
                    FGroup.FoundTextInfos.Add(FText);
                }
                SInfo.Groups.Add(FGroup);
            }
            return DSites.SInfos;
        }

        private static FoundTextInfo GetTextFromHtmlDocument(HtmlDocument HDoc, AnalyzedElement AElement)
        {
            List<HtmlNode> PossibleMainNodes = GetPossibleMainNodes(HDoc, AElement);
            List<HtmlNode> MainNodes = ConfirmMainNodes(PossibleMainNodes, AElement);
            return GetTextFromHtmlNodes(MainNodes, AElement);
        }

        private static List<HtmlNode> GetPossibleMainNodes(HtmlDocument HDoc, AnalyzedElement AElement)
        {
            if (AElement.XPath.VariablePathPieces.Any(x => x.PPiece == PathPiece.Variable))
            {
                List<HtmlNode> PossibleMainNodes = new List<HtmlNode>();
                BetterRecursive(AElement, HDoc, ref PossibleMainNodes, 0, String.Empty);
                return PossibleMainNodes;
            }
            try
            {
                return new List<HtmlNode>() { HDoc.DocumentNode.SelectSingleNode(AElement.MainNode.XPath) };
            }
            catch (Exception)
            {
                // need some kind of error here to tell the user that no nodes with the correct path was found
                return new List<HtmlNode>();
            }
        }

        private static void BetterRecursive(AnalyzedElement AElement, HtmlDocument HDoc, ref List<HtmlNode> PossibleMainNodes, int StartIndex, string XPath)
        {
            const char XPathSplitter = '/';
            List<string> SplittedPath = AElement.MainNode.XPath.Split(XPathSplitter).ToList();
            SplittedPath.RemoveAll(x => x == String.Empty);
            for (int i = StartIndex; i < AElement.XPath.VariablePathPieces.Count; i++)
            {
                if (AElement.XPath.VariablePathPieces[i].PPiece == PathPiece.Constant)
                {
                    XPath += XPathSplitter + SplittedPath[i];
                }
                else
                {
                    const string ReplacementString = "!x!";
                    string XPathPeice = Regex.Replace(SplittedPath[i], @"\d+", ReplacementString);
                    HtmlNode HNode = HDoc.DocumentNode.SelectSingleNode(XPath);
                    for (int y = 0; y < HNode.ChildNodes.Count; y++)
                    {
                        int NewStartIndex = i + 1;
                        string NewXPath = XPath + XPathSplitter + Regex.Replace(XPathPeice, ReplacementString, y.ToString());
                        BetterRecursive(AElement, HDoc, ref PossibleMainNodes, NewStartIndex, NewXPath);
                    }
                    //returns so the variable node isn't added to the list of possible main nodes
                    return;
                }
            }
            HtmlNode ToAdd = HDoc.DocumentNode.SelectSingleNode(XPath);
            if (ToAdd != null)
            {
                PossibleMainNodes.Add(ToAdd);
            }
        }

        private static List<HtmlNode> ConfirmMainNodes(List<HtmlNode> PossibleMainNodes, AnalyzedElement AElement)
        {
            List<HtmlNode> ConfirmedMainNodes = (from x in PossibleMainNodes
                                                 where Tools.IsSameNode(x,AElement.MainNode)
                                                 select x).ToList();
            return ConfirmedMainNodes;
        }

        private static FoundTextInfo GetTextFromHtmlNodes(List<HtmlNode> MainNodes, AnalyzedElement AElement)
        {
            FoundTextInfo FInfo = new FoundTextInfo(); 
            foreach (HtmlNode MainNode in MainNodes)
            {
                StringBuilder SBuilder = new StringBuilder();
                foreach (HtmlNode HNode in MainNode.Descendants())
                {
                    if (HNode.Name != "#text")
                    {
                        continue;
                    }
                    if (AElement.WhiteList.Any(x => Tools.IsSameNode(x,HNode.ParentNode)))
                    {
                        SBuilder.Append(HNode.InnerText);
                    }
                }
                FInfo.FoundText.Add(SBuilder.ToString());
            }
            return FInfo;
        }
    }

    public class ProcessedInfo
    {

    }
}

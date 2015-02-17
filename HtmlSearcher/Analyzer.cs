using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace HtmlSearcher
{
    public static class Analyzer
    {
        internal static AnalyzedDocument AnalyseDocument(HtmlDocument HDoc, ScrapInfo WebInfo)
        {
            AnalyzedDocument ADoc = new AnalyzedDocument();
            ADoc.HDoc = HDoc;
            foreach (GroupToFind GInfo in WebInfo.Groups)
            {
                foreach (TextToFindInfo TInfo in GInfo.TextsToFind)
                {
                    switch (TInfo.TextAmountToFind)
                    {
                        case TextAmount.Multiple:
                            ADoc.AElements.Add(GetElementsWithMultipleTexts(ADoc.HDoc, TInfo));
                            break;
                        case TextAmount.Single:
                            ADoc.AElements.Add(GetElementsWithSingleText(ADoc.HDoc, TInfo));
                            break;
                    }
                }
            }
            return ADoc;
        }

        private static AnalyzedElement GetElementsWithSingleText(HtmlDocument HDoc, TextToFindInfo TInfo)
        {
            return FindTextInHtmlNode(HDoc, TInfo.TextsToFind.First());
        }

        private static AnalyzedElement GetElementsWithMultipleTexts(HtmlDocument HDoc, TextToFindInfo TInfo)
        {
            List<AnalyzedElement> AInfos = new List<AnalyzedElement>();
            TInfo.TextsToFind.ForEach(x => AInfos.Add(FindTextInHtmlNode(HDoc, x)));

            AnalyzedElement AElement = new AnalyzedElement();
            AElement.MainNode = AInfos.First().MainNode; // not the best solution. need to compare them and generalise the MainNode
            AElement.WhiteList = GetCombinedWhitelistedNodes(AInfos);
            AElement.XPath = GetVariablePath(AInfos);
            return AElement;
        }

        private static List<HtmlNode> GetCombinedWhitelistedNodes(List<AnalyzedElement> AElements)
        {
            List<HtmlNode> WhiteListedNodes = new List<HtmlNode>();
            foreach (AnalyzedElement AElement in AElements)
            {
                foreach (HtmlNode HNode in AElement.WhiteList)
                {
                    if (!WhiteListedNodes.Any(x => Tools.IsSameNode(x, HNode)))
                    {
                        WhiteListedNodes.Add(HNode);
                    }
                }
            }
            return WhiteListedNodes;
        }

        private static AnalyzedElement FindTextInHtmlNode(HtmlDocument HDoc, string ToFind)
        {
            AnalyzedElement AInfo = GetNodesUsed(HDoc, ToFind);
            AInfo.MainNode = GetMainNode(HDoc, AInfo);
            return AInfo;
        }

        private static AnalyzedElement GetNodesUsed(HtmlDocument HDoc, string ToFind)
        {
            StringBuilder SBuilder = new StringBuilder();
            List<HtmlNode> UsedNodes = new List<HtmlNode>();
            foreach (HtmlNode MainHtmlNode in HDoc.DocumentNode.Descendants())
            {
                foreach (HtmlNode HNode in MainHtmlNode.Descendants())
                {
                    if (HNode.Name != "#text")
                    {
                        continue;  
                    }
                    string CheckToAdd = HNode.InnerText.Replace(Environment.NewLine, String.Empty); // removes new lines. might have to remove other things in the future

                    //makes sure the string to add is not longer than the space left in the setence to find
                    if (SBuilder.Length + CheckToAdd.Length <= ToFind.Length)
                    {
                        string CheckToMatch = ToFind.Substring(SBuilder.Length, CheckToAdd.Length);
                        if (CheckToMatch == CheckToAdd && CheckToAdd.Length > 0)
                        {
                            SBuilder.Append(CheckToAdd);
                            UsedNodes.Add(HNode.ParentNode);
                        }
                    }
                    else
                    {
                        if (ToFind == SBuilder.ToString())
                        {
                            return new AnalyzedElement(UsedNodes);
                        }
                        break;
                    }
                }
                SBuilder.Clear();
                UsedNodes.Clear();
            }
            throw new ExpectedException("Couldn't Find: " + ToFind + " In The Code");
        }

        private static HtmlNode GetMainNode(HtmlDocument HDoc, AnalyzedElement AInfo)
        {
            HtmlNode MainNodeCandidate = AInfo.WhiteList[0];

            while (MainNodeCandidate != null)
            {
                if (AInfo.WhiteList.All(x => NodeDescendeantFamilyContainsNode(MainNodeCandidate, x)))
                {
                    return MainNodeCandidate;
                }
                MainNodeCandidate = MainNodeCandidate.ParentNode;
            }
            throw new ExpectedException("Found No MainNode");
        }

        private static bool NodeDescendeantFamilyContainsNode(HtmlNode ContainerNode, HtmlNode ToFind)
        {
            if (Tools.IsSameNode(ContainerNode, ToFind))
            {
                return true;
            }
            foreach (HtmlNode ChildNode in ContainerNode.ChildNodes)
            {
                if (NodeDescendeantFamilyContainsNode(ChildNode, ToFind))
                {
                    return true;
                }
            }
            return false;
        }

        private static VariablePath GetVariablePath(List<AnalyzedElement> AInfos)
        {
            List<List<string>> PathNumbers = GetPathNumbers(AInfos);
            VariablePath VPath = MakeVariablePath(PathNumbers);
            return VPath;
        }

        private static List<List<string>> GetPathNumbers(List<AnalyzedElement> AInfos)
        {
            List<List<string>> PathNumbers = new List<List<string>>();
            foreach (AnalyzedElement AInfo in AInfos)
            {
                List<string> PathNumber = new List<string>();
                List<string> PathFragments = AInfo.MainNode.XPath.Split('/').ToList();
                PathFragments.RemoveAll(x => x == String.Empty);
                PathFragments.ForEach(x => PathNumber.Add(Regex.Replace(x, "[^.0-9]", String.Empty)));
                PathNumbers.Add(PathNumber);
            }
            return PathNumbers;
        }

        private static VariablePath MakeVariablePath(List<List<string>> PathsNumbers)
        {
            VariablePath VPath = new VariablePath();
            if (!PathsNumbers.All(x => PathsNumbers.All(z => z.Count == x.Count)))
            {
                throw new ExceptionNotImplementedYet("The length of the paths doesn't match up");
            }
            // makes the variable path by going through each number and comparing them
            for (int i = 0; i < PathsNumbers.First().Count; i++)
            {
                VariablePathPiece VPathPiece = new VariablePathPiece();
                //if all the numbers with the index "i" is the same
                if (PathsNumbers.All(x => x[i] == PathsNumbers.First()[i]))
                {
                    VPathPiece.PPiece = PathPiece.Constant;
                }
                else
                {
                    VPathPiece.PPiece = PathPiece.Variable;
                }
                VPath.VariablePathPieces.Add(VPathPiece);
            }
            return VPath;
        }
    }

    [Serializable]
    internal enum PathPiece
    {
        Constant,
        Variable
    }
    [Serializable]
    internal class VariablePathPiece
    {
        internal PathPiece PPiece;

        public override string ToString()
        {
            if (PPiece == PathPiece.Variable)
            {
                return "Variable";   
            }
            else
            {
                return "Constant";   
            }
        }
    }
    [Serializable]
    internal class VariablePath
    {
        internal List<VariablePathPiece> VariablePathPieces = new List<VariablePathPiece>();
    }



    internal class AnalyzedElement
    {
        
        //internal TextToFindInfo TInfo; // Multi Fixed
        internal List<HtmlNode> WhiteList;
        internal HtmlNode MainNode;
        internal VariablePath XPath;
                
        public AnalyzedElement(List<HtmlNode> NUsed)
        {
            WhiteList = NUsed;
        }
        public AnalyzedElement()
        {

        }
    }
    internal class AnalyzedDocument
    {
        internal List<AnalyzedElement> AElements = new List<AnalyzedElement>();
        internal HtmlDocument HDoc;
    }

}
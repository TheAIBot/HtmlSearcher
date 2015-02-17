using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;
using HtmlAgilityPack;

namespace HtmlSearcher
{
    public static class Tools
    {
        internal static void Opdateringer(string Text)
        {
            Search.SetUpdateEvent(Text);
        }

        internal static string RemoveHtmlCode(string HtmlLine)
        {
            bool HasStart = HtmlLine.Contains('>');
            bool HasEnd = HtmlLine.Contains('<');

            if (HasStart && !HasEnd)
            {
                return HtmlLine.Split('>')[1];
            }
            if (!HasStart && HasEnd)
            {
                return HtmlLine.Split('<')[0];
            }
            if (HasStart)
            {
                string[] HtmlLines = HtmlLine.Split('>');

                for (int i = 0; i < HtmlLines.Length; i++)
                {
                    if (HtmlLines[i].Contains('<'))
                    {
                        HtmlLines[i] = HtmlLines[i].Substring(0, HtmlLines[i].IndexOf('<'));
                    }
                }
                return String.Join(String.Empty, HtmlLines);
            }
            return HtmlLine;
        }

        public static void SaveAsXML<T>(T ToSave, string FilePath)
        {
            const string FileExtension = ".xml";
            FilePath = Path.ChangeExtension(FilePath, FileExtension);

            using (FileStream FStream = new FileStream(FilePath, FileMode.Create))
            {
                XmlSerializer Serializer = new XmlSerializer(typeof(T));
                Serializer.Serialize(FStream, ToSave);
            }
        }

        public static T LoadAsXML<T>(string FilePath)
        {
            const string FileExtension = ".xml";
            FilePath = Path.ChangeExtension(FilePath, FileExtension);

            using (FileStream FStream = new FileStream(FilePath,FileMode.Open))
            {
                XmlSerializer Serializer = new XmlSerializer(typeof(T));
                return (T)Serializer.Deserialize(FStream);
            }
        }

        public static void SaveAsBinary<T>(T ToSave, string FilePath)
        {
            const string FileExtension = ".bin";
            FilePath = Path.ChangeExtension(FilePath, FileExtension);

            using (FileStream FStream = new FileStream(FilePath, FileMode.Create))
            {
                BinaryFormatter Formatter = new BinaryFormatter();
                Formatter.Serialize(FStream, ToSave);
            }
        }

        public static T LoadAsBinary<T>(string FilePath)
        {
            const string FileExtension = ".bin";
            FilePath = Path.ChangeExtension(FilePath, FileExtension);

            using (FileStream FStream = new FileStream(FilePath, FileMode.Open))
            {
                BinaryFormatter Formatter = new BinaryFormatter();
                return (T)Formatter.Deserialize(FStream);
            }
            
        }

        internal static T DeepCopy<T>(T Other)
        {
            using (MemoryStream MStream = new MemoryStream())
            {
                BinaryFormatter Formatter = new BinaryFormatter();
                Formatter.Serialize(MStream, Other);
                MStream.Position = 0;
                return (T)Formatter.Deserialize(MStream);
            }
        }

        internal static bool IsSameNode(HtmlNode Node1, HtmlNode Node2)
        {
            if (Node1.Name == Node2.Name &&
                Node1.NodeType == Node2.NodeType &&
                Tools.SameAttributes(Node1.Attributes, Node2.Attributes))
            {
                return true;
            }
            return false;
        }

        internal static bool SameAttributes(HtmlAttributeCollection Ac1, HtmlAttributeCollection Ac2)
        {
            if (Ac1.Count != Ac2.Count)
            {
                return false;
            }
            return Ac1.All(x => Ac2.Contains(x.Name) && Ac2[x.Name].Value == x.Value);
        }
    }
}


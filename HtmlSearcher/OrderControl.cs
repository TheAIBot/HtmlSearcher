using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace HtmlSearcher
{
    public static class OrderControl
    {
        private const string DirectoryName = "Orders";

        private static string CreateFileName()
        {
            const string FileNamePrefix = "Order: ";
            List<string> OrderPaths = Directory.GetDirectories(DirectoryName).ToList();
            if (OrderPaths.Count > 0)
            {
                //Order Names are ordered as follows: "Order: {OrderNumber}"
                //the OrderNumber has to be isolated, so the new order can get a OrderNumber that doesn't match any other OrderNumber
                List<int> OrderNumbers = (from OrderPath in OrderPaths
                                        let Number = OrderPath.Split(' ')[1]
                                        select Convert.ToInt32(Number)).ToList();

                int MaxOrderNumber = OrderNumbers.Max();
                for (int i = 0; i < MaxOrderNumber; i++)
                {
                    if (!OrderNumbers.Contains(i))
                    {
                        return FileNamePrefix + i.ToString();
                    }
                }
                return FileNamePrefix + (MaxOrderNumber + 1).ToString();
            }
            // there is no order in the directory and the first order is 0
            return FileNamePrefix + "0";
        }

        public static Order CreateNewOrder(ScrapInfo FInfo)
        {
            string FileName = CreateFileName();
            string FilePath = Path.Combine(DirectoryName, FileName);
            return null;
        }
    }

    public class Order
    {
        private string DirectoryPath;
        private Queue<string> filenames = new Queue<string>();

        public Order(string MainOrderControlPath, string DirectoryName)
        {

        }

        public void InitializeSaving()
        {

        }

        public void SaveWebsite(string HtmlCode)
        {

        }

        public void EndSaving()
        {

        }
        public void GetWebsite()
        {

        }


    }

    public class SiteInfo
    {
        public string HtmlCode;
    }
}

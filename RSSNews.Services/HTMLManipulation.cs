using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSSNews.Services
{
    public class HTMLManipulation
    {
        public static List<string> SplitOffImgs(ref string strHtml)
        {
            var html = new HtmlDocument();
            html.LoadHtml(strHtml);
            List<string> anImgs = new List<string>();
            List<HtmlNode> toDel = new List<HtmlNode>();

            var root = html.DocumentNode;
            var anchors = root.Descendants("a");

            foreach (var item in anchors)
            {
                Console.WriteLine(item.OuterHtml);
                var imgs = item.Descendants("img");
                foreach (var img in imgs)
                {
                    Console.WriteLine(img.OuterHtml);
                }
                if (imgs.Count() > 0)
                {
                    anImgs.Add(item.OuterHtml);
                    toDel.Add(item);
                }
            }
            foreach (var item in toDel)
            {
                item.Remove();
            }
            toDel.Clear();

            var simgs = root.Descendants("img");
            foreach (var img in simgs)
            {
                Console.WriteLine(img.OuterHtml);
                anImgs.Add(img.OuterHtml);
                toDel.Add(img);
            }
            foreach (var item in toDel)
            {
                item.Remove();
            }

            strHtml = root.OuterHtml;
            return anImgs;
        }
    }
}

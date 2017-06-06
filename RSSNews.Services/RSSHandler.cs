using RSSNews.Models;
using RSSNews.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RSSNews.Services
{
    public class RSSHandler : IDisposable
    {
        private RSSNewsContext db = new RSSNewsContext();
        private Random Randomizer = new Random();

        public RSSHandler()
        {

        }
        public RSSHandler(RSSNewsContext Appdb)
        {
            db = Appdb;
        }

        public void AcquireNewsForSource(Source source)
        {
            XElement root = XElement.Load(source.Address);  //http://www.komputerswiat.pl/rss-feeds/komputer-swiat-feed.aspx  //http://www.tvn24.pl/najnowsze.xml
            var rssItems = root.Descendants("item").Select(n => new News
            {
                Title = n.Element("title").Value,
                Description = n.Element("description").Value,
                Category = n.Element("category")?.Value ?? "None",
                Link = n.Element("link").Value,
                PubDate = DateTime.Parse(n.Element("pubDate").Value, System.Globalization.CultureInfo.InvariantCulture),  //, System.Globalization.CultureInfo.InvariantCulture
                Source = source //db.Sources.First(m => m.Address == source)
            }).OrderByDescending(n => n.PubDate).ToList();

            var NewestRssItem = db.News.Where(n => n.Source.Address == source.Address).OrderByDescending(n => n.PubDate).FirstOrDefault();
            if (NewestRssItem == null)
            {
                db.News.AddRange(rssItems);
            }
            else
            {
                int i = 0;
                while (i < rssItems.Count && NewestRssItem.PubDate < rssItems[i].PubDate)
                {
                    db.News.Add(rssItems[i]);
                    i++;
                }
            }          

            //foreach (var item in rssItems)
            //{
            //    if (!db.News.Any(n => n.Title == item.Title))
            //    {
            //        db.News.Add(item);
            //    }
            //}

            db.SaveChanges();
        }

        public List<News> GetNewsForUser(int count, string uId, ref Dictionary<string, int> ONewsPerCategory)
        {
            int CT = 0; // CT = Chance Total;
            int CS = 0; // CS = Current Sum;
            Dictionary<string, int> likesPerCategory = new Dictionary<string, int>();
            Dictionary<string, int> NewsPerCategory = new Dictionary<string, int>();
            foreach (var source in db.Sources)
            {
                if (!likesPerCategory.ContainsKey(source.Category))
                {
                    likesPerCategory.Add(source.Category,
                        (db.UserLikesCategories.FirstOrDefault(n => n.UserId == uId && n.Category == source.Category))?.
                        Likes ?? 1);
                    CT += likesPerCategory[source.Category];

                    NewsPerCategory.Add(source.Category, 0);
                }
            }
       
            for (int i = 0; i < count; i++)
            {
                int R = Randomizer.Next(CT);
                int F = CT - R; // F = Final Random Value;
                CS = 0;
                foreach (var item in likesPerCategory)
                {
                    CS += item.Value;
                    if (F <= CS)
                    {
                        NewsPerCategory[item.Key] += 1;
                        break;
                    }
                }                         
            }

            List<News> toReturn = new List<News>();
            if (ONewsPerCategory == null)
            {
                foreach (var category in NewsPerCategory)
                {
                    toReturn.AddRange(db.News.Where(n => n.Source.Category == category.Key).OrderByDescending(n => n.PubDate).Take(category.Value));
                }
                ONewsPerCategory = NewsPerCategory;
            }
            else
            {
                foreach (var category in NewsPerCategory)
                {
                    toReturn.AddRange(db.News.Where(n => n.Source.Category == category.Key).OrderByDescending(n => n.PubDate).Skip(ONewsPerCategory[category.Key]).Take(category.Value));
                    ONewsPerCategory[category.Key] += category.Value;
                }
            }

            if (toReturn.Count <= 0 && NewsPerCategory.Count > 0 && db.News.Count() > 0)
            {
                ONewsPerCategory = null;
                return GetNewsForUser(count, uId, ref ONewsPerCategory);
            }
            return toReturn;
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public void LikeNews(string uId, int nId)
        {
            string category = (db.News.First(n => n.Id == nId)).Source.Category;
            var likes = db.UserLikesCategories.FirstOrDefault(n => n.UserId == uId && n.Category == category) ??
                new UserLikesCategory {Category = category, UserId = uId, Id = -1};

            likes.Likes++;
            db.UserLikesCategories.AddOrUpdate(likes);
            db.SaveChanges();
        }
    }
}

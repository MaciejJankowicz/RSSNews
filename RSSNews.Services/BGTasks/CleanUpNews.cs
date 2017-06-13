using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using RSSNews.Repository;
using RSSNews.Models;
using System.Configuration;

namespace RSSNews.Services.BGTasks
{
    public class CleanUpNews : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            using (RSSNewsContext db = new RSSNewsContext())
            {
                DateTime maxDateOk = DateTime.UtcNow.AddDays(-(Int32.Parse(ConfigurationManager.AppSettings["DaysTillOldNews"])));
                var ToDel = db.News.Where(n => n.PubDate < maxDateOk);
                if (ToDel.Count() > 0)
                {
                    db.News.RemoveRange(ToDel);
                    db.SaveChanges();
                }
                
                List<News> Replaced = new List<News>();
                foreach (var newsItem in db.News)
                {
                    Replaced.AddRange( db.News.Where(n => n.Title == newsItem.Title && n.PubDate < newsItem.PubDate) );
                }
                if (Replaced.Count > 0)
                {
                    db.News.RemoveRange(Replaced);
                    db.SaveChanges();
                }
                
            }
        }
    }
}

using Quartz;
using RSSNews.Models;
using RSSNews.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSSNews.Services.BGTasks
{
    public class AcquireNews : IJob
    {

        public void Execute(IJobExecutionContext context)
        {
            using (RSSNewsContext db = new RSSNewsContext())
            using (RSSHandler servs = new RSSHandler(db))
            {
                foreach (Source source in db.Sources.ToList())
                {
                    servs.AcquireNewsForSource(source);
                }
            }
        }
    }
}


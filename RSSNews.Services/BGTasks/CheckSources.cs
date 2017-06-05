using Quartz;
using RSSNews.Repository;
using RSSNews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;

namespace RSSNews.Services.BGTasks
{
    public class CheckSources : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            using (RSSNewsContext db = new RSSNewsContext())
            {
                SourcesXMLHandler xml = new SourcesXMLHandler(context.JobDetail.JobDataMap.GetString("XMLPath"));

                var approvedSources = xml.GetEntries();
                if (approvedSources == null)
                {
                    return;
                }

                // Delete sources not in XML
                var ToDel = db.Sources.ToList().Where(n => !approvedSources.Exists(a => a.Address == n.Address));
                if (ToDel.Count() > 0)
                {
                    db.Sources.RemoveRange(ToDel);
                    db.SaveChanges();
                }

                // Add sources and Modify categories if needed
                db.Sources.AddOrUpdate(p => p.Address, approvedSources.Select(n => new Source
                {
                    Address = n.Address,
                    Category = n.Category
                }).ToArray());
                db.SaveChanges();
            }
        }
    }
}

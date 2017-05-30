using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace RSSNews.Repository
{
    public class RSSNewsContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public System.Data.Entity.DbSet<RSSNews.Models.News> News { get; set; }
        public System.Data.Entity.DbSet<RSSNews.Models.Source> Sources { get; set; }
        public System.Data.Entity.DbSet<RSSNews.Models.UserLikesCategory> UserLikesCategories { get; set; }
        

        public RSSNewsContext() : base("name=RSSNewsContext")
        {
            Database.SetInitializer(new NewsInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RSSNews.Models.News>().
                HasRequired(s => s.Source).
                WithMany(n => n.News).
                WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }

    }

    public class NewsInitializer : DropCreateDatabaseIfModelChanges<RSSNewsContext>
    {
        protected override void Seed(RSSNewsContext context)
        {
            context.Sources.AddOrUpdate(
               p => p.Address,
               new Models.Source
               {
                   Address = "http://www.komputerswiat.pl/rss-feeds/komputer-swiat-feed.aspx",
                   Category = "Technologia"
               }, new Models.Source
               {
                   Address = "http://www.tvn24.pl/najnowsze.xml",
                   Category = "Informacje"
               });

            base.Seed(context);
        }
    }
   
}

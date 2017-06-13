using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RSSNews.Models;
using RSSNews.Repository;
using RSSNews.Services;
using Microsoft.AspNet.Identity;
using System.IO;

namespace RSSNews.Controllers
{
    public static class RazorViewToString
    {
        public static string RenderRazorViewToString(this Controller controller, string viewName, object model)
        {
            controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, controller.ViewData, controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controller.ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }

    public class NewsController : Controller
    {
        private RSSNewsContext db;
        private RSSHandler servs;
        private Random Randomizer = new Random();
        public NewsController()
        {
            db = new RSSNewsContext();
            servs = new RSSHandler(db);
        }

        // GET: News
        public ActionResult Index()
        {
            //foreach (Source source in db.Sources.ToList())
            //{
            //    servs.AcquireNewsForSource(source);
            //}

            if (User.Identity.IsAuthenticated)
            {
                TempData["NewsSent"] = null;
                var NewsSent = (Dictionary<string, int>)(TempData["NewsSent"]);
                var ToView = servs.GetNewsForUser(10, User.Identity.GetUserId(), ref NewsSent);
                TempData["NewsSent"] = NewsSent;
                return View(ToView);
            }
            else
            {
                return View(db.News.OrderByDescending(n => n.PubDate).Take(10).ToList());
            }
            
        }

        public string GetRows(int count)
        {
            if (TempData["lastRowsDeliveredTime"] == null)
            {
                TempData["lastRowsDeliveredTime"] = DateTime.MinValue;
            }
            DateTime lastRowsDeliveredTime = (DateTime)TempData["lastRowsDeliveredTime"];
            if (lastRowsDeliveredTime > DateTime.Now.AddMilliseconds(-500) )
            {
                TempData["lastRowsDeliveredTime"] = lastRowsDeliveredTime;
                return "";
            }
                   
            string toSend = "";
            if (User.Identity.IsAuthenticated)
            {
                var NewsSent = (Dictionary<string, int>)(TempData["NewsSent"]);
                var ToView = servs.GetNewsForUser(count*2, User.Identity.GetUserId(), ref NewsSent);
                TempData["NewsSent"] = NewsSent;

                for (int i = 0; i < count; i++)
                {
                    
                    if (ToView.Count >(i*2 + 2))
                    {
                        toSend += RazorViewToString.RenderRazorViewToString(this, "RowPartial", ToView.Skip(i * 2).Take(2));
                    }                   
                }
                TempData["lastRowsDeliveredTime"] = DateTime.Now;
                return toSend;
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    int m, n;
                    m = Randomizer.Next(db.News.Count());
                    n = Randomizer.Next(db.News.Count());
                    var lDb = db.News.ToList();
                    List<News> lN = new List<News> { lDb.ElementAt(m), lDb.ElementAt(n) };
                    toSend += RazorViewToString.RenderRazorViewToString(this, "RowPartial", lN);
                }
                TempData["lastRowsDeliveredTime"] = DateTime.Now;
                return toSend;  
            }
        } 

        [HttpPost]
        public void LikeNews(int id)
        {
           var uId = User.Identity.GetUserId();
            servs.LikeNews(uId, id);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (servs == null)
                {
                    db.Dispose();
                }
                else
                {
                    servs.Dispose();
                }                               
            }
            base.Dispose(disposing);
        }
    }
}

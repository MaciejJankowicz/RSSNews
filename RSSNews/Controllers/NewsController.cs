﻿using System;
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

namespace RSSNews.Controllers
{
    public class NewsController : Controller
    {
        private RSSNewsContext db;
        private RSSHandler servs;
        public NewsController()
        {
            db = new RSSNewsContext();
            servs = new RSSHandler(db);
        }

        // GET: News
        public ActionResult Index()
        {
            foreach (Source source in db.Sources.ToList())
            {
                servs.AcquireNewsForSource(source);
            }

            if (User.Identity.IsAuthenticated)
            {
                return View(servs.GetNewsForUser(10, User.Identity.GetUserId()));
            }
            else
            {
                return View(db.News.OrderByDescending(n => n.PubDate).Take(10).ToList());
            }
            
        }

        // GET: News/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // GET: News/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: News/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Link,PubDate,Category")] News news)
        {
            if (ModelState.IsValid)
            {
                db.News.Add(news);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(news);
        }

        // GET: News/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: News/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,Description,Link,PubDate,Category")] News news)
        {
            if (ModelState.IsValid)
            {
                db.Entry(news).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(news);
        }

        // GET: News/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = db.News.Find(id);
            if (news == null)
            {
                return HttpNotFound();
            }
            return View(news);
        }

        // POST: News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            News news = db.News.Find(id);
            db.News.Remove(news);
            db.SaveChanges();
            return RedirectToAction("Index");
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

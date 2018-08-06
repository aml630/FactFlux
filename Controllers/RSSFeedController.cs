using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FactFlux.Controllers
{
    public class RSSFeedController : Controller
    {
        FactFluxEntities db = new FactFluxEntities();
        // GET: RSSFeed
        public ActionResult Index()
        {
            var feedList = db.RSSFeeds.ToList();

            return View(feedList);
        }

        // GET: RSSFeed/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // POST: RSSFeed/Create
        public ActionResult Create(string title, string link, string image, string feedType)
        {
            using (var db = new FactFluxEntities())
            {
                var newFeed = new RSSFeed
                {
                    FeedTitle = title,
                    FeedLink = link,
                    SourceImage = image,
                    FeedType=feedType
                };
                db.RSSFeeds.Add(newFeed);
                db.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        // GET: RSSFeed/Edit/5
        public ActionResult Update(int RssId, string title, string link, string image)
        {
            using (var db = new FactFluxEntities())
            {
                var newFeed = db.RSSFeeds.FirstOrDefault(x => x.FeedId == RssId);

                newFeed.FeedTitle = title;
                newFeed.FeedLink = link;
                newFeed.SourceImage = image;

                db.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        public int ResourceCount(int feedId)
        {
            using (var db = new FactFluxEntities())
            {
                 int newFeed = db.Resources.Where(x => x.FeedId == feedId).Count();
                return newFeed;
            }
        }

        // GET: RSSFeed/Delete/5
        public ActionResult Delete(int id)
        {
            var findResources = db.Resources.Where(x => x.FeedId == id);
            foreach(var resource in findResources)
            {
                db.Resources.Remove(resource);
            }

            var findFeed = db.RSSFeeds.FirstOrDefault(x => x.FeedId == id);

            db.RSSFeeds.Remove(findFeed);

            db.SaveChanges();

            return Redirect(Request.UrlReferrer.ToString());
        }
    }
}

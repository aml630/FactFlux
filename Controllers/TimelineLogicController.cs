using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace FactFlux.Controllers
{
    public class TimelineLogicController : Controller
    {
        // GET: Timeline

        public ActionResult LoadTimelineFromWord(string word)
        {
            Word searchWord;

            var cachedTimeline = MemoryCache.Default["timelineResources_" + word];

            List<ArticleLink> orderedResources;


            FactFluxEntities db = new FactFluxEntities();

            string wordWithSpace = word.Replace('-', ' ').ToLower();

            if (cachedTimeline != null)
            {
                searchWord = db.Words.Where(x => x.Word1.ToLower() == wordWithSpace).FirstOrDefault();
                orderedResources = (List<ArticleLink>)cachedTimeline;
            }
            else
            {
                searchWord = db.Words.Where(x => x.Word1.ToLower() == wordWithSpace).FirstOrDefault();

                var totalRecordsWithChildrenWords = db.ParentWords.Where(x => x.ParentWordId == searchWord.WordId).Select(x => x.ChildWordId).ToList();

                totalRecordsWithChildrenWords.Add(searchWord.WordId);

                var wordLogs = (from t1 in db.WordLogs
                                join t2 in totalRecordsWithChildrenWords on t1.WordId equals t2
                                select t1);

                wordLogs = wordLogs.GroupBy(x => x.ArticleLinkId).Select(group => group.FirstOrDefault());

                orderedResources = (from t1 in db.ArticleLinks
                                    join t2 in wordLogs on t1.ArticleLinkId equals t2.ArticleLinkId
                                    select t1).OrderByDescending(y => y.DatePublished).ToList();

                MemoryCache.Default["timelineResources_" + word] = orderedResources;
            }

            ViewBag.MainWord = searchWord;

            return View("TimeLine", orderedResources);
        }

        public ActionResult AddTimeline(string title, string image, string slug)
        {
            FactFluxEntities db = new FactFluxEntities();

            Timeline timLin = new Timeline();

            timLin.TimelineImage = image;
            timLin.TimelineTitle = title;
            timLin.TimelineSlug = slug;
            timLin.Active = false;
            timLin.LastUpdated = DateTime.Now;

            db.Timelines.Add(timLin);
            db.SaveChanges();

            return RedirectToAction("Index", "Home");
        }

        public void AddSocialShare(string timelineType, int timelineId)
        {
            using (FactFluxEntities db = new FactFluxEntities())
            {
                Timeline foundTimeLine = db.Timelines.FirstOrDefault(x => x.TimelineId == timelineId);

                if (timelineType == "twitter")
                {
                    foundTimeLine.TwShare += 1;
                }

                if (timelineType == "facebook")
                {
                    foundTimeLine.FbShare += 1;
                }

                db.SaveChanges();
            }
        }

        public ActionResult ChangeImage(string slug, string newImage)
        {
            using (FactFluxEntities context = new FactFluxEntities())
            {
                Timeline foundTimeLine = context.Timelines.FirstOrDefault(x => x.TimelineSlug == slug);

                foundTimeLine.TimelineImage = newImage;

                context.SaveChanges();
            }
            return RedirectToAction("LoadTimeline", "TimelineLogic", new { slug = slug });
        }

        [HttpPost]
        public ActionResult UploadImage(string slug, HttpPostedFileBase imageFile)
        {
            //you can put your existing save code here
            if (imageFile != null && imageFile.ContentLength > 0)
            {
                WebImage img = new WebImage(imageFile.InputStream);

                img.Resize(600, 400);

                string filePath = Path.Combine(Server.MapPath("/Content/TimelineImages"), Path.GetFileName(imageFile.FileName));

                img.Save(filePath);

                string filePresentLocation = "/Content/TimelineImages/" + imageFile.FileName;

                ChangeImage(slug, filePresentLocation);
            }

            return RedirectToAction("LoadTimeline", "TimelineLogic", new { slug = slug });
        }

        public ActionResult DeleteTimeline(int timelineId)
        {
            using (FactFluxEntities context = new FactFluxEntities())
            {
                Timeline foundTimeLine = context.Timelines.FirstOrDefault(x => x.TimelineId == timelineId);

                var resources = context.Resources.Where(x => x.TimelineId == foundTimeLine.TimelineId).ToList();

                var searchWOrds = context.SearchWords.Where(x => x.TimelineId == foundTimeLine.TimelineId).ToList();

                foreach (var word in searchWOrds)
                {
                    context.SearchWords.Remove(word);
                }

                foreach (var res in resources)
                {
                    context.Resources.Remove(res);
                }

                context.Timelines.Remove(foundTimeLine);
                context.SaveChanges();
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult UnPublishTimeline(int timelineId)
        {
            using (FactFluxEntities context = new FactFluxEntities())
            {
                Timeline foundTimeLine = context.Timelines.FirstOrDefault(x => x.TimelineId == timelineId);

                foundTimeLine.Active = false;
                context.SaveChanges();
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult PublishTimeline(int timelineId)
        {
            using (FactFluxEntities context = new FactFluxEntities())
            {
                Timeline foundTimeLine = context.Timelines.FirstOrDefault(x => x.TimelineId == timelineId);

                foundTimeLine.Active = true;
                context.SaveChanges();
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult LoadOldTimeline(string slug)
        {
            Timeline findTimeLine;
            FactFluxEntities db = new FactFluxEntities();

            findTimeLine = db.Timelines.Include("Resources").FirstOrDefault(x => x.TimelineSlug == slug);

            var orderedResources = db.Resources.Where(x => x.TimelineId == findTimeLine.TimelineId && x.Active == true).OrderByDescending(y => y.DatePublished).ToList();

            ViewBag.Resources = orderedResources;

            ViewBag.SearchWords = db.SearchWords.Where(x => x.TimelineId == findTimeLine.TimelineId).ToList();

            return View(findTimeLine);
        }

    }
}
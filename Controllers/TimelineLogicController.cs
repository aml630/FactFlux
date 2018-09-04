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

        public ActionResult LoadTimelineFromWord(string wordSlug, int howMany = 100000, int articleType = 1, int startingAt = 0, string searchPhrase = "")
        {
            if (searchPhrase == "") { searchPhrase = null; }

            Word searchWord;

            using (FactFluxEntities db = new FactFluxEntities())
            {
                var cachedTimeline = MemoryCache.Default["timelineResources_" + wordSlug];

                searchWord = db.Words.Where(x => x.Slug == wordSlug).FirstOrDefault();

                var orderedResources = new List<GetTimeline_Result>();

                ViewBag.MainWord = searchWord;

                if (cachedTimeline != null)
                {
                    orderedResources = (List<GetTimeline_Result>)cachedTimeline;

                    return View("TimeLine", orderedResources);
                }

                var orderedResourcesQuery = db.GetTimeline(wordSlug, howMany, articleType, startingAt, searchPhrase);

                orderedResources = orderedResourcesQuery.ToList();

                MemoryCache.Default["timelineResources_" + wordSlug] = orderedResources;

                return View("TimeLine", orderedResources);
            }
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
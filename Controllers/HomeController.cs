using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FactFlux.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            List<Timeline> context;

            using (FactFluxEntities db = new FactFluxEntities())
            {
                context = db.Timelines.OrderByDescending(x => x.LastUpdated).ToList();
            }

            return View(context);
            //return RedirectToAction("WordsInTheNews", "Word");

        }

        public int ArticleCount(int timelineID)
        {
            using (var db = new FactFluxEntities())
            {
                int articleCount = db.Resources.Where(x => x.TimelineId == timelineID).Count();
                return articleCount;
            }
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
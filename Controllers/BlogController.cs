using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FactFlux.Controllers
{
    public class BlogController : Controller
    {
        // GET: Post
        public ActionResult TrumpRussia()
        {
            return View("~/Views/Blog/trump-russia.cshtml");
        }

        public ActionResult OrangeGraph()
        {

            return View();
        }

        public ActionResult GraphDataCard()
        {

            return View("~/Views/Blog/Graphs/GraphDataCard.cshtml");
        }

        public ActionResult TransparentBarGraph()
        {

            return View();
        }

        public ActionResult ColorfulDataPoints()
        {

            return View();
        }
    }
}
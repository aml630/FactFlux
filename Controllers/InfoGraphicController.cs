using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FactFlux.Controllers
{
    public class InfoGraphicController : Controller
    {
        // GET: InfoGraphic
        public ActionResult Index()
        {
            return View("Graphic1");
        }
    }
}
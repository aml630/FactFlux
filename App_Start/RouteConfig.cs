using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FactFlux
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

                            routes.MapRoute(
                name: "HomePageWordsInNews",
                url: "",
                defaults: new { controller = "Word", action = "WordsInTheNews" }
                );


            routes.MapRoute(
                name: "WordsInTheNews_today",
                url: "today",
                defaults: new { controller = "Word", action = "WordsInTheNews", timeFrame = "today" }
                );


            routes.MapRoute(
                name: "WordsInTheNews_week",
                url: "past-week",
                defaults: new { controller = "Word", action = "WordsInTheNews", timeFrame = "week" }
                );


            routes.MapRoute(
                name: "WordsInTheNews_month",
                url: "past-month",
                defaults: new { controller = "Word", action = "WordsInTheNews", timeFrame = "month" }
                );

            routes.MapRoute(
                name: "WordsInTheNews_year",
                url: "past-year",
                defaults: new { controller = "Word", action = "WordsInTheNews", timeFrame = "year" }
                );

                        routes.MapRoute(
                name: "About",
                url: "about",
                defaults: new { controller = "Home", action = "About"}
                );


            routes.MapRoute(
                name: "WordsInTheNews",
                url: "WordsInTheNews/{timeFrame}",
                defaults: new { controller = "Word", action = "WordsInTheNews", timeFrame = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "LoadTimeline",
                url: "timeline/{slug}",
                defaults: new { controller = "TimelineLogic", action = "LoadTimeline", slug = UrlParameter.Optional }
            );

                    routes.MapRoute(
            name: "InfoGraphic",
            url: "Infographic/{slug}",
            defaults: new { controller = "InfoGraphic", action = "index", slug = UrlParameter.Optional }
        );




            routes.MapRoute(
                name: "LoadTimelineFromWord",
                url: "{word}",
                defaults: new { controller = "TimelineLogic", action = "LoadTimelineFromWord", word = UrlParameter.Optional }
            );

                        routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "CheckRecentWords",
                url: "CheckRecentWords/{daysBack}",
                defaults: new { controller = "Word", action = "CheckRecentWords", daysBack = UrlParameter.Optional }
                );
        }
    }
}

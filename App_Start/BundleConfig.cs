using System.Web;
using System.Web.Optimization;

namespace FactFlux
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/NewJS/RecentPost.js",
                      "~/Scripts/NewJS/Site.js"
                      ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/Site.css",
                      "~/Content/NewCSS/RecentPost.css",
                      "~/Content/NewCSS/PostLink.css"
                      ));

            //////////////////////////////////Post1 Load ins 

            bundles.Add(new StyleBundle("~/Post1/css").Include(
                  "~/Content/NewCSS/TimeLine.css",
                  "~/Content/NewCSS/YouTubeEmbed.css",
                  "~/Content/NewCSS/Banner.css",
                      "~/Content/Site.css"
                  ));
            bundles.Add(new ScriptBundle("~/Post1/js").Include(
                 "~/Scripts/NewJS/YouTubeEmbed.js",
                 "~/Scripts/NewJS/Banner.js"
            ));


            //////////////////////////////////Post1 Load ins 

            //////////////////////////////////GraphDataCard//////


            bundles.Add(new StyleBundle("~/GraphDataCard/css").Include(
                  "~/Content/NewCSS/GraphDataCard.css"
                  ));
            bundles.Add(new ScriptBundle("~/GraphDataCard/js").Include(
                      "~/Scripts/NewJS/GraphDataCard.js"));

        }
    }
}

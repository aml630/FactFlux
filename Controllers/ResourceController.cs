using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Timers;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace FactFlux.Controllers
{
    public class ResourceController : Controller
    {
        /// <summary>
        //These 3 methods allow the links to be updated every half hour
        /// </summary>
        private static System.Timers.Timer aTimer;

        public void StartArticlePullDownTimer()
        {
            // Create a timer with a 3o minute interval.
            aTimer = new System.Timers.Timer(1800000);
            //aTimer = new System.Timers.Timer(5000);
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            CheckFeedsForResources();
        }

        public ActionResult AddYoutube(string YouTubeEmbed, string title, DateTime datePublished, int timelineId, string slug)
        {
            //WebClient w = new WebClient();
            //string s = w.DownloadString(YouTubeEmbed);

            //int pFrom = s.IndexOf("Published on ") + "Published on ".Length;
            //int pTo = s.LastIndexOf(" </strong></div> ");

            //string date = s.Substring(pFrom, pTo - pFrom);

            string toBeSearched = "watch?v=";
            string result = YouTubeEmbed.Substring(YouTubeEmbed.IndexOf(toBeSearched) + toBeSearched.Length);
            AddSingleResource(title, result, datePublished, timelineId, 3, 3);

            return RedirectToAction("LoadTimeline", "TimelineLogic", new { slug = slug });
        }

        [ValidateInput(false)]
        public void AddTwitter(string TwitterEmbed, DateTime datePublished, int timeLineId)
        {
            AddSingleResource("", "", datePublished, timeLineId, 4, 4, TwitterEmbed);
        }

        [ValidateInput(false)]
        public static void AddSingleResource(string title, string link, DateTime datepublished, int timelineId, int feedid = 2
            , int resourceType = 1, string factText = "")
        {
            using (var db = new FactFluxEntities())
            {
                Resource newResource = new Resource();
                newResource.ResourceTitle = title;
                newResource.ResourceUrl = link;
                newResource.DateAdded = DateTime.Now;
                newResource.DatePublished = datepublished;
                newResource.ResourceType = resourceType;
                newResource.TimelineId = timelineId;
                newResource.FeedId = feedid;
                newResource.Active = true;
                newResource.FactText = factText;

                db.Resources.Add(newResource);

                var foundtimeLIne = db.Timelines.FirstOrDefault(x => x.TimelineId == timelineId);

                foundtimeLIne.LastUpdated = DateTime.Now;

                db.SaveChanges();
            }
        }

        public static void NoteLastUpdated(int RssID)
        {
            using (var db = new FactFluxEntities())
            {
                var newFeed = db.RSSFeeds.FirstOrDefault(x => x.FeedId == RssID);

                newFeed.LastUpdated = DateTime.Now;

                db.SaveChanges();
            }
        }



        public static void PullDownLinksFromFeeds(RSSFeed rssFeed, List<SearchWord> searchwords)
        {
            if (rssFeed.FeedLink != null)
            {
                try
                {
                    NoteLastUpdated(rssFeed.FeedId);

                    var r = XmlReader.Create(rssFeed.FeedLink);
                    var albums = SyndicationFeed.Load(r);

                    //for each article in the feed
                    foreach (var item in albums.Items)
                    {
                        var text = item.Title.Text;

                        //for each word in the article title, see if it contains one of the strings we're searching for

                        foreach (var searchWord in searchwords)
                        {
                            if (text.ToLower().Contains(searchWord.SearchWordString.ToLower()))
                            {
                                //If we do find a match, and we're only looking for a single word, add the article if its unique
                                if (string.IsNullOrEmpty(searchWord.SearchWordString2))
                                {
                                    if (OnlyAddUniqueArticle(item.Title.Text, searchWord.TimelineId))
                                    {
                                        AddSingleResource(item.Title.Text, item.Links[0].Uri.AbsoluteUri, item.PublishDate.UtcDateTime, searchWord.TimelineId, rssFeed.FeedId);
                                    }
                                }
                                //If we do find a match, and we're looking for multiple words, search for the other word as well
                                else
                                {
                                    //If you find a connection to the other word, add the article if its unique
                                    if (text.ToLower().Contains(searchWord.SearchWordString2.ToLower()))
                                    {
                                        if (OnlyAddUniqueArticle(item.Title.Text, searchWord.TimelineId))
                                        {
                                            AddSingleResource(item.Title.Text, item.Links[0].Uri.AbsoluteUri, item.PublishDate.UtcDateTime, searchWord.TimelineId, rssFeed.FeedId);
                                        }

                                    }
                                }
                            }
                        }
                    }
                    r.Close();
                }
                catch (Exception ex)
                {

                }
            }
        }

        public ActionResult CheckFeedsForResources()
        {
            using (FactFluxEntities db = new FactFluxEntities())
            {
                var searchwords = db.SearchWords.ToList();

                var feeds = db.RSSFeeds.ToList();

                foreach (var feed in feeds)
                {
                    PullDownLinksFromFeeds(feed, searchwords);
                }
            }

            return RedirectToAction("Index", "Home");
        }

        public ActionResult CheckFeedsForResourcesForSingleTimeline(int timelineId, string slug)
        {
            using (FactFluxEntities db = new FactFluxEntities())
            {
                var searchwords = db.SearchWords.Where(x => x.TimelineId == timelineId).ToList();

                var feeds = db.RSSFeeds.ToList();

                foreach (var feed in feeds)
                {
                    PullDownLinksFromFeeds(feed, searchwords);
                }
            }

            return RedirectToAction("LoadTimeline", "TimelineLogic", new { slug = slug});
        }


        public ActionResult AddSearchWord(string searchWord, int timeLineId, string searchWord2 = "")
        {
            using (FactFluxEntities db = new FactFluxEntities())
            {
                var timeLine = db.Timelines.FirstOrDefault(x => x.TimelineId == timeLineId);

                SearchWord newWord = new SearchWord
                {
                    SearchWordString = searchWord,
                    SearchWordString2 = searchWord2,
                    TimelineId = timeLineId
                };

                db.SearchWords.Add(newWord);
                db.SaveChanges();


                return RedirectToAction("LoadTimeline", "TimelineLogic", new { slug = timeLine.TimelineSlug });
            }
        }

        public ActionResult DeleteSearchWord(int searchWordId, string slug)
        {
            using (FactFluxEntities db = new FactFluxEntities())
            {
                var foundWord = db.SearchWords.FirstOrDefault(x => x.SearchWordId == searchWordId);

                db.SearchWords.Remove(foundWord);
                db.SaveChanges();
            }
            return RedirectToAction("LoadTimeline", "TimelineLogic", new { slug = slug });
        }

        public static bool OnlyAddUniqueArticle(string articleTitle, int timelineId)
        {
            using (FactFluxEntities db = new FactFluxEntities())
            {
                var repeats = db.Resources.FirstOrDefault(x => x.ResourceTitle == articleTitle && x.TimelineId == timelineId);

                if (repeats == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public ActionResult DeactivateResource(int resourceId, string slug)
        {
            using (FactFluxEntities db = new FactFluxEntities())
            {
                var resource = db.Resources.FirstOrDefault(x => x.ResourceId == resourceId);

                resource.Active = false;

                db.SaveChanges();

            }

            return RedirectToAction("LoadTimeline", "TimelineLogic", new { slug = slug });
        }

        public ActionResult AddTimelineFact(string factText, DateTime factDate, int timelineId, string factSource, string slug)
        {
            AddSingleResource(factText, factSource, factDate, timelineId, 1, 2);

            return RedirectToAction("LoadTimeline", "TimelineLogic", new { slug = slug });
        }

    }
}
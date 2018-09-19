using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using FactFlux.Models;
using System.Runtime.Caching;
using Google.Apis.YouTube.v3.Data;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Web.Helpers;
using System.IO;
using FactFlux.Logic;

namespace FactFlux.Controllers
{
    public class WordController : Controller
    {
        public ActionResult MostDiscussed()
        {
            using (FactFluxEntities db = new FactFluxEntities())
            {
                var topDaily = db.Words
                              .Where(x => x.Banned == false &&
                              !db.ParentWords.Select(y => y.ChildWordId).Contains(x.WordId))
                              .OrderByDescending(x => x.DailyCount).Take(20).Select(x => new WordApiWordInfo()
                              { Word = x.Word1, Slug = x.Slug, Image = x.Image, DailyCount = x.DailyCount.Value, WeeklyCount = x.WeeklyCount.Value, MonthlyCount = x.MonthlyCount.Value, YearlyCount = x.YearlyCount.Value, WordId=x.WordId})
                              .ToList();

                return View("Front", topDaily);
            }
        }

        private static List<GetWordsWithCount_Result> GetWordsWithCountInternal(string timeFrame)
        {
            DateTime AddedSinceDate = DateTime.Now;

            AddedSinceDate = ChangedAddedSince(timeFrame, AddedSinceDate);

            List<GetWordsWithCount_Result> wordsWithCount;

            using (FactFluxEntities db = new FactFluxEntities())
            {
                wordsWithCount = db.GetWordsWithCount(AddedSinceDate).Where(x => x.Count > 1).OrderByDescending(x => x.Count).ToList();
            }

            return wordsWithCount;
        }

        public string WakeUp()
        {
            return "I'm awake jeese";
        }

        public ActionResult AddImageToWord(int wordId, HttpPostedFileBase imageFile)
        {
            using (FactFluxEntities db = new FactFluxEntities())
            {
                var getWord = db.Words.Where(x => x.WordId == wordId).FirstOrDefault();
                getWord.Image = UploadImage(imageFile);
                db.SaveChanges();
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        public string UploadImage(HttpPostedFileBase imageFile)
        {
            string filePresentLocation = "";

            if (imageFile != null && imageFile.ContentLength > 0)
            {
                WebImage img = new WebImage(imageFile.InputStream);

                img.Resize(600, 400);

                var savePath = AppDomain.CurrentDomain.BaseDirectory + "/Content/Images/WordImages/";

                string filePath = Path.Combine(savePath, Path.GetFileName(imageFile.FileName));

                img.Save(filePath);

                filePresentLocation = "/Content/Images/WordImages/" + imageFile.FileName;
            }

            return filePresentLocation;
        }

        private static DateTime ChangedAddedSince(string timeFrame, DateTime AddedSinceDate)
        {
            if (timeFrame == "all")
            {
                AddedSinceDate = AddedSinceDate.AddYears(-5);
            }

            if (timeFrame == "today")
            {
                AddedSinceDate = AddedSinceDate.AddDays(-1);
            }
            if (timeFrame == "week")
            {
                AddedSinceDate = AddedSinceDate.AddDays(-7);
            }
            if (timeFrame == "month")
            {
                AddedSinceDate = AddedSinceDate.AddMonths(-1);
            }
            if (timeFrame == "year")
            {
                AddedSinceDate = AddedSinceDate.AddYears(-1);
            }

            return AddedSinceDate;
        }

        public ActionResult AddParentWordOverChild(string parentWord, int childWordId)
        {
            using (FactFluxEntities db = new FactFluxEntities())
            {
                var parent = db.Words.Where(x => x.Word1.ToLower() == parentWord.ToLower()).FirstOrDefault();

                if (parent == null)
                {
                    parent = CreateNewWord(db, DateTime.Now, parentWord);
                }

                var parentWordRecord = new ParentWord();

                parentWordRecord.ParentWordId = parent.WordId;

                parentWordRecord.ChildWordId = childWordId;

                db.ParentWords.Add(parentWordRecord);

                db.SaveChanges();

                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        public ActionResult AddChildWord(int parentWordId, string childWord)
        {
            using (FactFluxEntities db = new FactFluxEntities())
            {
                var child = db.Words.Where(x => x.Word1.ToLower() == childWord.ToLower()).FirstOrDefault();

                if (child == null)
                {
                    child = CreateNewWord(db, DateTime.Now, childWord);
                }

                var parentWord = new ParentWord();

                parentWord.ParentWordId = parentWordId;

                parentWord.ChildWordId = child.WordId;

                db.ParentWords.Add(parentWord);

                db.SaveChanges();

                return Redirect(Request.UrlReferrer.ToString());
            }
        }


        public ActionResult Cores(int timeFrame = -1)
        {
            using (FactFluxEntities db = new FactFluxEntities())
            {
                var words = db.Words.Where(x => x.Banned == false);

                var popularWords = words.GroupBy(j => j.Word1).Where(g => g.Count() > 1).SelectMany(group => group).ToList();

                ViewBag.Articles = db.ArticleLinks.ToList();

                return View(popularWords);
            }
        }

        public ActionResult CheckRecentWords(int daysBack)
        {
            using (FactFluxEntities db = new FactFluxEntities())
            {
                DateTime start = DateTime.Now.AddDays(-daysBack);
                DateTime end = DateTime.Now.AddDays(-daysBack + 1);

                var words = db.Words.Where(x => x.Banned == false
                && x.DateCreated > start && x.DateCreated < end)
                .OrderBy(x => x.DateCreated).ToList();

                return View(words);
            }
        }

        public ActionResult Ban(int wordId)
        {
            using (FactFluxEntities db = new FactFluxEntities())
            {
                var word = db.Words.Where(x => x.WordId == wordId).FirstOrDefault();

                word.Banned = true;

                db.SaveChanges();

                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        public ActionResult UpdateWord(int wordId, string newWord)
        {
            using (FactFluxEntities db = new FactFluxEntities())
            {
                var word = db.Words.Where(x => x.WordId == wordId).FirstOrDefault();

                word.Word1 = newWord;

                //delete links
                var deleteList = db.WordLogs.Where(x => x.WordId == wordId);

                foreach (var log in deleteList)
                {
                    db.WordLogs.Remove(log);
                }

                //find new matching articles
                var articleList = db.ArticleLinks.Where(x => x.ArticleLinkTitle.Contains(newWord)).ToList();

                foreach (var matchingArticle in articleList)
                {
                    CreateWordLog(db, matchingArticle.DatePublished, matchingArticle.ArticleLinkId, word.WordId, newWord);
                }

                db.SaveChanges();

                return Redirect(Request.UrlReferrer.ToString());
            }
        }

        public string CheckFeedsForWords(bool singleSearch = true, int feedToSearch = -1)
        {

            string updateResults = "UpdateResults: ";

            using (FactFluxEntities db = new FactFluxEntities())
            {
                var feeds = db.RSSFeeds.Where(x => x.FeedId > 4).ToList();

                if (feedToSearch != -1)
                {
                    var feed = feeds.Where(x => x.FeedId == feedToSearch).FirstOrDefault();

                    updateResults = UpdateFeed(updateResults, db, feed);
                }
                else if (singleSearch)
                {
                    var feed = feeds.OrderBy(x => x.LastUpdated).FirstOrDefault();

                    updateResults = UpdateFeed(updateResults, db, feed);
                }
                else
                {
                    foreach (var feed in feeds)
                    {
                        updateResults = UpdateFeed(updateResults, db, feed);
                    }

                }
            }

            return updateResults;
        }

        private static string UpdateFeed(string updateResults, FactFluxEntities db, RSSFeed feed)
        {

            try
            {
                int articlesAdded = 0;

                if (feed.FeedType == "RSS")
                {
                    articlesAdded = AddNewResourcesFromRSS(ref updateResults, db, feed);
                }

                if (feed.FeedType == "YouTube")
                {
                    AddNewResourcesFromYouTube(ref updateResults, db, feed, ref articlesAdded);
                }

                feed.LastUpdated = DateTime.UtcNow;

                db.SaveChanges();

                updateResults += "\r\n---------------Feed Num: " + feed.FeedTitle.ToString() + " New Articles: " + articlesAdded + Environment.NewLine;
            }
            catch (Exception ex)
            {
                updateResults += "\r\n---------------Feed Num: " + feed.FeedTitle.ToString() + " Error: " + ex.Message + Environment.NewLine;

                feed.LastUpdated = DateTime.UtcNow;

                db.SaveChanges();
            }

            return updateResults;
        }

        private static void AddNewResourcesFromYouTube(ref string updateResults, FactFluxEntities db, RSSFeed feed, ref int articlesAdded)
        {
            var youTubeLogic = new Logic.YouTubeLogic();

            var videoList = youTubeLogic.GetVidsForFeed(feed.FeedLink);

            var vidListResult = videoList.Result.Where(x => x.Id.VideoId != null).ToList();

            foreach (var item in vidListResult)
            {
                var text = item.Snippet.Title;

                if (OnlyAddUniqueArticle(text))
                {
                    try
                    {
                        if (item.Snippet.PublishedAt == null)
                        {
                            item.Snippet.PublishedAt = DateTime.UtcNow;
                        }

                        ArticleLink newArticleLinke = CreateNewArticleLink(db, feed, item.Snippet.Title, item.Id.VideoId, item.Snippet.PublishedAt.Value);

                        articlesAdded += 1;
                    }
                    catch (Exception ex)
                    {
                        updateResults += ex.Message;
                    }
                }
            }
        }

        private static int AddNewResourcesFromRSS(ref string updateResults, FactFluxEntities db, RSSFeed feed)
        {
            var r = XmlReader.Create(feed.FeedLink);
            var albums = SyndicationFeed.Load(r);

            int articlesAdded = 0;

            var currentRSSArticeList = new List<string>();

            //for each article in the feed
            foreach (var item in albums.Items)
            {
                var text = item.Title.Text;

                var isArticleRepeatInFeedList = currentRSSArticeList.FirstOrDefault(stringToCheck => stringToCheck.Contains(text));

                if (OnlyAddUniqueArticle(text) && isArticleRepeatInFeedList == null)
                {
                    try
                    {
                        currentRSSArticeList.Add(text);

                        if (string.IsNullOrEmpty(item.Title.Text) || string.IsNullOrEmpty(item.Links[0].Uri.AbsoluteUri) || item.PublishDate.UtcDateTime == null)
                        {
                            break;
                        }

                        ArticleLink newArticleLinke = CreateNewArticleLink(db, feed, item.Title.Text, item.Links[0].Uri.AbsoluteUri, item.PublishDate.UtcDateTime);

                        articlesAdded += 1;
                    }
                    catch (DbEntityValidationException ex)
                    {
                        updateResults += ex.Message;
                    }
                }
            }
            r.Close();
            return articlesAdded;
        }

        private static ArticleLink CreateNewArticleLink(FactFluxEntities db, RSSFeed feed, string Title, string Url, DateTime publishDate)
        {
            var newArticleLinke = new ArticleLink();

            if (Title.Contains("&#8216;"))
            {
                Title = Title.Replace("&#8216;", "");
            }
            if (Title.Contains("&#8217;"))
            {
                Title = Title.Replace("&#8217;", "");
            }

            newArticleLinke.ArticleLinkTitle = Title;
            newArticleLinke.ArticleLinkUrl = Url;
            newArticleLinke.DatePublished = publishDate;
            newArticleLinke.DateAdded = DateTime.UtcNow;
            newArticleLinke.FeedId = feed.FeedId;

            db.ArticleLinks.Add(newArticleLinke);

            db.SaveChanges();
            return newArticleLinke;
        }

        public ActionResult ClearCache()
        {
            List<string> cacheKeys = MemoryCache.Default.Select(kvp => kvp.Key).ToList();
            foreach (string cacheKey in cacheKeys)
            {
                MemoryCache.Default.Remove(cacheKey);
            }
            return RedirectToAction("Index", "Home");
        }

        public string CutLatestArticleIntoWords()
        {
            using (var db = new FactFluxEntities())
            {
                var mostRecentArticle = db.ArticleLinks.SqlQuery(
                    "select * from " +
                    "articlelinks al left join wordlogs wl on wl.ArticleLinkId = al.ArticleLinkId " +
                    "where wl.wordlogid is null order by DatePublished desc")
                    .FirstOrDefault();

                return CutArticleIntoWords(db, mostRecentArticle);
            }      
        }


        private static string CutArticleIntoWords(FactFluxEntities db, ArticleLink newArticleLinke)
        {

            string responseText = "Chopping: " + newArticleLinke.ArticleLinkTitle;
            //divide article title into words
            var punctuation = newArticleLinke.ArticleLinkTitle.Where(Char.IsPunctuation).Distinct().ToArray();
            var words = newArticleLinke.ArticleLinkTitle.Split().Select(x => x.Trim(punctuation));

            var oneWordLogPerArticle = new List<string>();

            foreach (var singleWord in words)
            {
                //for each word, check if we store a word or phrase that contains it
                var doesExistList = db.Words.Where
                       (x => x.Word1.ToLower() == singleWord.ToLower()
                    || (x.Word1.Contains(" ") && x.Word1.ToLower().Contains(singleWord.ToLower())
                    )).OrderByDescending(x => x.Word1.Length).ToList();

                // if we dont haveit, make it
                if (doesExistList == null || doesExistList.Count == 0 || !doesExistList.Any(x => x.Word1.ToLower() == singleWord.ToLower()) && singleWord != "")
                {
                    responseText += "--Creating: " + singleWord;
                    var newWord = CreateNewWord(db, newArticleLinke.DatePublished, singleWord);
                }

                //if we do have it, lets loop through them
                foreach (var doesExist in doesExistList)
                {
                    //words or phrases should only be logged once per article
                    var alreadyLogged = oneWordLogPerArticle.Any(x => x.Contains(newArticleLinke.ArticleLinkId.ToString() + doesExist.WordId));
                    //if the word isn't banned, and the entire word or phrase is contained in the article, lets log it
                    if (doesExist.Banned == false && newArticleLinke.ArticleLinkTitle.ToLower().Contains(doesExist.Word1.ToLower()) && alreadyLogged == false)
                    {
                        responseText += "--logging: " + singleWord;

                        oneWordLogPerArticle.Add(newArticleLinke.ArticleLinkId.ToString() + doesExist.WordId);

                        CreateWordLog(db, newArticleLinke.DatePublished, newArticleLinke.ArticleLinkId, doesExist.WordId, doesExist.Slug);
                    }
                }

                db.SaveChanges();
            }
            return responseText;
        }

        private static void CreateWordLog(FactFluxEntities db, DateTime datePublished, int articleId, int wordId, string wordSlug)
        {
            WordLog newWordLog = new WordLog();
            newWordLog.WordId = wordId;
            newWordLog.DateAdded = datePublished;
            newWordLog.ArticleLinkId = articleId;

            var findParentWord = db.ParentWords.Where(x => x.ChildWordId == wordId).FirstOrDefault();

            if (findParentWord != null)
            {
                var parentWordSlug = findParentWord.Word.Slug;
                MemoryCache.Default.Remove("timelineResources_" + parentWordSlug);
            }
            else
            {
                MemoryCache.Default.Remove("timelineResources_" + wordSlug);
            }

            db.WordLogs.Add(newWordLog);

            db.SaveChanges();

        }

        private static Word CreateNewWord(FactFluxEntities db, DateTime item, string singleWord)
        {
            var newWord = db.Words.Where(x => x.Word1.ToLower() == singleWord.ToLower()).FirstOrDefault();

            if (newWord == null && singleWord != "")
            {
    
                string sqLQuery = "select * from articlelinks al where " +
                    "(al.ArticleLinkTitle like '%[^a-zA-Z0-9]' + '"+singleWord+"' + '[^a-zA-Z0-9]%' " +
                    "or al.ArticleLinkTitle like '' + '" + singleWord + "' + ' %' " +
                    "or al.ArticleLinkTitle like '% ' + '" + singleWord + "' + '') ";

                var articleList = db.ArticleLinks.SqlQuery(sqLQuery)
                    .ToList();

                newWord = new Word();

                newWord.Word1 = singleWord;
                newWord.DateCreated = item;
                newWord.DailyCount = articleList.Where(x=>x.DatePublished > DateTime.Now.AddDays(-1)).Count();
                newWord.WeeklyCount = articleList.Where(x => x.DatePublished > DateTime.Now.AddDays(-7)).Count();
                newWord.MonthlyCount = articleList.Where(x => x.DatePublished > DateTime.Now.AddDays(-30)).Count();
                newWord.YearlyCount = articleList.Where(x => x.DatePublished > DateTime.Now.AddDays(-365)).Count();
                newWord.Slug = singleWord.Replace(" ", "-").ToLower();

                var newCreatedWord = db.Words.Add(newWord);

                db.SaveChanges();

                foreach (var article in articleList)
                {
                    CreateWordLog(db, article.DatePublished, article.ArticleLinkId, newWord.WordId, singleWord);
                }

                db.SaveChanges();
            }

            return newWord;
        }

        public Word CreateNewWordInternal(string singleWord)
        {
            using (FactFluxEntities db = new FactFluxEntities())
            {
                return CreateNewWord(db, DateTime.Now, singleWord);
            }
        }

        public ActionResult CreateNewWordWithChild(string singleWord, string childWord = "")
        {
            var newWord = CreateNewWordInternal(singleWord);

            if (!string.IsNullOrEmpty(childWord))
            {
                var addChild = AddChildWord(newWord.WordId, childWord);
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult CheckFeedsForResources()
        {
            using (FactFluxEntities db = new FactFluxEntities())
            {
                var feeds = db.RSSFeeds.ToList();

                foreach (var feed in feeds)
                {
                    PullDownAllLinksFromFeeds(feed);
                }
            }

            return Redirect(Request.UrlReferrer.ToString());
        }

        public static void PullDownAllLinksFromFeeds(RSSFeed rssFeed)
        {
            if (rssFeed.FeedLink != null)
            {
                var r = XmlReader.Create(rssFeed.FeedLink);
                var albums = SyndicationFeed.Load(r);

                foreach (var item in albums.Items)
                {
                    var text = item.Title.Text;

                    if (OnlyAddUniqueArticle(text))
                    {
                        using (FactFluxEntities db = new FactFluxEntities())
                        {
                            try
                            {
                                var newArticleLinke = new ArticleLink();

                                newArticleLinke.ArticleLinkTitle = item.Title.Text;
                                newArticleLinke.ArticleLinkUrl = item.Links[0].Uri.AbsoluteUri;
                                newArticleLinke.DatePublished = item.PublishDate.UtcDateTime;
                                newArticleLinke.DateAdded = DateTime.UtcNow;
                                newArticleLinke.FeedId = rssFeed.FeedId;

                                db.ArticleLinks.Add(newArticleLinke);

                                db.SaveChanges();
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                }

                r.Close();

            }

        }

        public static bool OnlyAddUniqueArticle(string articleTitle)
        {
            using (FactFluxEntities db = new FactFluxEntities())
            {
                var repeats = db.ArticleLinks.FirstOrDefault(x => x.ArticleLinkTitle == articleTitle);

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

        public string ReRunWordCounts()
        {
            using (FactFluxEntities db = new FactFluxEntities())
            {
                db.Database.CommandTimeout = 480;

                db.ReRunWordCounts();
            }

            return "Success";
        }

        public string GetWordsThatContainInput(string containsLetters)
        {

            using (FactFluxEntities db = new FactFluxEntities())
            {
                var listOfWords = db.Words.Where(x => x.Word1.Contains(containsLetters)
                && x.Banned == false
                & x.DailyCount != null)
                .Select(x => new WordApiWordInfo()
                { Word = x.Word1, Slug = x.Slug, DailyCount = x.DailyCount.Value, WeeklyCount = x.WeeklyCount.Value, MonthlyCount = x.MonthlyCount.Value, YearlyCount = x.YearlyCount.Value }).Take(50).ToList();

                var json = JsonConvert.SerializeObject(listOfWords);

                return json;
            }
        }

        public string GetTimeFrameWords(int timeFrame, int? pageNumber)
        {
            using (FactFluxEntities db = new FactFluxEntities())
            {
                var childWordList = db.ParentWords.Select(x => x.ChildWordId).Distinct();

                var listOfWords = db.Words.Where(x => x.Banned == false && !childWordList.Contains(x.WordId));


                if (timeFrame == 1)
                {
                    listOfWords = listOfWords.Where(x => x.DailyCount != null).OrderByDescending(x => x.DailyCount);
                }

                if (timeFrame == 2)
                {
                    listOfWords = listOfWords.Where(x => x.WeeklyCount != null).OrderByDescending(x => x.WeeklyCount);
                }

                if (timeFrame == 3)
                {
                    listOfWords = listOfWords.Where(x => x.MonthlyCount != null).OrderByDescending(x => x.MonthlyCount);
                }

                if (timeFrame == 4)
                {
                    listOfWords = listOfWords.Where(x => x.YearlyCount != null).OrderByDescending(x => x.YearlyCount);
                }


                if (pageNumber != null)
                {
                    listOfWords = listOfWords.Skip(20 * pageNumber.Value);
                }

                var listToSend = listOfWords.Take(20).Select(x => new WordApiWordInfo()
                { Word = x.Word1, Slug = x.Slug, Image = x.Image, DailyCount = x.DailyCount.Value, WeeklyCount = x.WeeklyCount.Value, MonthlyCount = x.MonthlyCount.Value, YearlyCount = x.YearlyCount.Value }).ToList();

                var json = JsonConvert.SerializeObject(listToSend);

                return json;
            }
        }
    }
}
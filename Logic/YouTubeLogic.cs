using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Web;

namespace FactFlux.Logic
{
    public class YouTubeLogic
    {
        public async Task<List<SearchResult>> GetVidsForFeed(string channelId)
        {
            var youtubeService = new YouTubeService(new BaseClientService.Initializer()
            {
                ApiKey = ConfigurationManager.AppSettings["AdminName"],
                ApplicationName = this.GetType().ToString()
            });

            var searchListRequest = youtubeService.Search.List("snippet");

            searchListRequest.MaxResults = 20;
            searchListRequest.ChannelId = channelId;
            searchListRequest.Order = 0;

            // Call the search.list method to retrieve results matching the specified query term.
            var searchListResponse = await searchListRequest.ExecuteAsync().ConfigureAwait(false);

            var listOfVids = new List<SearchResult>(searchListResponse.Items);

            return listOfVids;
        }
    }
}
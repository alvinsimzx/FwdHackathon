using Firebase.Auth;
using Firebase.Storage;
using FwdHackathon.Areas.Identity.Data;
using FwdHackathon.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refit;
using System.Diagnostics;
using System.Text.Json.Nodes;
using Tweetinvi;
using Tweetinvi.Client;
using Tweetinvi.Models;
using Tweetinvi.Models.V2;

namespace FwdHackathon.Controllers
{
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;
    private readonly TwitterClient _userClient;

    public HomeController(ILogger<HomeController> logger, TwitterClient userClient)
    {
      _logger = logger;
      _userClient = userClient;
    }

    public async Task<IActionResult> Index()
    {
      var homeTimelineResult = await _userClient.Execute.RequestAsync(request =>
      {
        request.Url = "https://api.twitter.com/1.1/trends/place.json?id=23424901";
        request.HttpMethod = Tweetinvi.Models.HttpMethod.GET;
      });

      var usersClient = RestService.For<IClassifier>("https://api.uclassify.com/v1/uclassify");
      var jsonResponse = homeTimelineResult.Content;
      var unQuotedString = jsonResponse.TrimStart('[').TrimEnd(']');
      TrendsList model = JsonConvert.DeserializeObject<TrendsList>(unQuotedString);

      int counter = 0;
      model.trends = model.trends.Take(5).ToList();
      List<double> listOfMatch = new List<double>();
      List<string> listOfCategories = new List<string>();

      foreach (Trend t in model.trends)
      {
        Dictionary<string, double> users = await usersClient.GetMatches(t.name);
        users = users.OrderByDescending(i => i.Value).ToDictionary(pair => pair.Key, pair => pair.Value);
        listOfMatch.Add(users.ElementAt(0).Value * 100);
        listOfCategories.Add(users.ElementAt(0).Key);

      }

      ViewBag.listOfMatch = listOfMatch;
      ViewBag.listOfCategories = listOfCategories;

      return View(model.trends);
    }

    public async Task<JsonResult> getTweets(string hashtag)
    {
      ITweet[] tweetsIterator = await _userClient.Search.SearchTweetsAsync("#" + hashtag);
      List<ITweet> listTweets = new List<ITweet>(tweetsIterator);
      List<Tweet> customTweets = new List<Tweet>();
      foreach (ITweet tweet in listTweets)
      {
        Tweet newTweet = new Tweet(tweet.FullText, tweet.CreatedBy.Name);
        customTweets.Add(newTweet);
      }


      customTweets = customTweets.Take(5).ToList();
      return Json(customTweets);
    }

    public IActionResult Upload()
    {
      return View();
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> Upload(string categoryDict)
    {
      var test = categoryDict;

      return Ok();
    }

    public IActionResult Trends()
    {
      return View();
    }

    public IActionResult Privacy()
    {
      return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
      return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
  }
}
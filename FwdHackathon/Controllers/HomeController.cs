using Firebase.Auth;
using Firebase.Storage;
using FwdHackathon.Areas.Identity.Data;
using FwdHackathon.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.MSIdentity.Shared;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Refit;
using System.Diagnostics;
using System.Security.Principal;
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
    private readonly AppDbContext _appDbContext;

    public HomeController(ILogger<HomeController> logger, TwitterClient userClient, AppDbContext appDbContext)
    {
      _logger = logger;
      _userClient = userClient;
      _appDbContext = appDbContext;
    }

    public async Task<IActionResult> Index()
    {
      var homeTimelineResult = await _userClient.Execute.RequestAsync(request =>
      {
        request.Url = "https://api.twitter.com/1.1/trends/place.json?id=23424901";
        request.HttpMethod = Tweetinvi.Models.HttpMethod.GET;
      });

      var jsonResponse = homeTimelineResult.Content;
      var unQuotedString = jsonResponse.TrimStart('[').TrimEnd(']');
      TrendsList model = JsonConvert.DeserializeObject<TrendsList>(unQuotedString);

      int counter = 0;
      model.trends = model.trends.Take(5).ToList();
      List<double> listOfMatch = new List<double>();
      List<string> listOfCategories = new List<string>();

      var usersClient = RestService.For<IClassifier>("https://api.uclassify.com/v1/uclassify");

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
    public async Task<JsonResult> getMatches(string word)
    {
      var usersClient = RestService.For<IClassifier>("https://api.uclassify.com/v1/uclassify");
      Dictionary<string, double> users = await usersClient.GetMatches(word);
      users = users.OrderByDescending(i => i.Value).ToDictionary(pair => pair.Key, pair => pair.Value);

      var categoryList = _appDbContext.TransData
        .Where(i => i.Label == users.ElementAt(0).Key)
        .Select(i => i.Value);

      return Json();
    }

    public IActionResult Upload()
    {
      return View();
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> Upload(string jsonResponse)
    {
      Category model = JsonConvert.DeserializeObject<Category>(jsonResponse);

      var usersClient = RestService.For<IClassifier>("https://api.uclassify.com/v1/uclassify");

      Dictionary<string, double> users = await usersClient.GetMatches(model.key);
      users = users.OrderByDescending(i => i.Value).ToDictionary(pair => pair.Key, pair => pair.Value);


      _appDbContext.Add(new TransData(model.key, model.value, users.ElementAt(0).Key));
      await _appDbContext.SaveChangesAsync();

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
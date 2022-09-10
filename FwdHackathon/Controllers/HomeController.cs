using Firebase.Auth;
using Firebase.Storage;
using FwdHackathon.Areas.Identity.Data;
using FwdHackathon.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.Json.Nodes;
using Tweetinvi;
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

      var jsonResponse = homeTimelineResult.Content;
      var unQuotedString = jsonResponse.TrimStart('[').TrimEnd(']');
      TrendsList model = JsonConvert.DeserializeObject<TrendsList>(unQuotedString);

      int counter = 0;
      model.trends = model.trends.Take(5).ToList();
      foreach (Trend t in model.trends)
      {
        if (counter > 4)
        {
          model.trends.Remove(t);
        }
        else
        {
          counter += 1;
        }

      }

      return View(model.trends);
    }

    public async Task<JsonResult> getTweets(string hashtag)
    {

      var tweets = await _userClient.Execute.RequestAsync(request =>
      {
        request.Url = "https://api.twitter.com/1.1/search/tweets.json?q=%23" + hashtag;
        request.HttpMethod = Tweetinvi.Models.HttpMethod.GET;
      });

      var jsonResponse = tweets.Content;
      var unQuotedString = jsonResponse.TrimStart('[').TrimEnd(']');

      return Json(unQuotedString);
    }

    public IActionResult Upload()
    {
      return View();
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> Upload(IFormFile files)
    {
      //authentication
      var auth = new FirebaseAuthProvider(new FirebaseConfig("AIzaSyDWm3HA3AqJQ3OTRlY3H6L_mxWhLmgKILU"));
      var a = await auth.SignInWithEmailAndPasswordAsync("anthonyleehj@gmail.com", "abcd1234");

      // Constructr FirebaseStorage, path to where you want to upload the file and Put it there
      var task = new FirebaseStorage(
          "fwd-hack-2022.appspot.com",
           new FirebaseStorageOptions
           {
             AuthTokenAsyncFactory = () => Task.FromResult(a.FirebaseToken),
             ThrowOnCancel = true,
           })
          .Child("data")
          .Child("random")
          .Child("file.csv")
          .PutAsync(files.OpenReadStream());

      // Track progress of the upload
      task.Progress.ProgressChanged += (s, e) => Console.WriteLine($"Progress: {e.Percentage} %");

      // await the task to wait until upload completes and get the download url
      var downloadUrl = await task;

      // load the csv file into memory
      var data = files.OpenReadStream();

      // create a dictionary of category : count
      Dictionary<string, string> categories = new Dictionary<string, string>(); 

      // pass the dictionary of category : count to the trends page

      // redirect to trends page
      return RedirectToAction(nameof(Index));
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
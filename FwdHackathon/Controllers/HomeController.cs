using FwdHackathon.Areas.Identity.Data;
using FwdHackathon.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.Json.Nodes;
using Tweetinvi;

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
            foreach(Trend t in model.trends)
            {
                if (counter>4)
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

    public IActionResult Upload()
    {
      return View();
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
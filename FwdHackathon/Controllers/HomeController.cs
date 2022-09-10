using FwdHackathon.Areas.Identity.Data;
using FwdHackathon.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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
        request.Url = "https://api.twitter.com/1.1/trends/place.json?id=1";
        request.HttpMethod = Tweetinvi.Models.HttpMethod.GET;
      });

      var jsonResponse = homeTimelineResult.Content;

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
using FwdHackathon.Areas.Identity.Data;
using FwdHackathon.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FwdHackathon.Controllers
{
  public class HomeController : Controller
  {
    private readonly ILogger<HomeController> _logger;
    private readonly ITwitterData _twitterData;

    public HomeController(ILogger<HomeController> logger, ITwitterData twitterData)
    {
      _logger = logger;
      _twitterData = twitterData;
    }

    public IActionResult Index()
    {
      var testing = _twitterData.GetTweets(1);

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
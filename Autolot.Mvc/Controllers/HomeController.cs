using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Services.Logging;
using WebApplication1.Models;

namespace WebApplication1.Controllers;

public class HomeController : Controller
{
    private readonly IAppLogging<HomeController> _logger;

    public HomeController(/*IAppLogging<HomeController> logger*/)
    {
        /*
        _logger = logger;
    */
    }

    public IActionResult Index()
    {
        /*
        _logger.LogAppWarning("this is a test");
        */
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
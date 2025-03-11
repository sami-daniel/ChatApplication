using ChatApplication.Web.Data;
using Microsoft.AspNetCore.Mvc;

namespace ChatApplication.Web.Controllers;

[Route("")]
[Route("home")]
public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Route("")]
    public IActionResult Index()
    {
        return View();
    }

}

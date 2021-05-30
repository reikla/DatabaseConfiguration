using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace WebApplication
{
  public class ApiController : Controller
  {
    private readonly SgSystemConfiguration _config;
    public ApiController(IOptions<SgSystemConfiguration> options)
    {
      _config = options.Value;
    }
    
    // GET
    public IActionResult Index()
    {
      return Json(_config);
    }
  }
}
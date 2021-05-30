using DatabaseConfiguration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace WebApplication
{
  [ApiController]
  [Route("api/v1/[controller]")]
  public class ConfigurationController : Controller
  {
    private readonly IOptionsMonitor<SgSystemConfiguration> _monitor;
    private readonly IConfiguration _configuration;
    private readonly SgSystemConfiguration _config;
    public ConfigurationController(IOptionsMonitor<SgSystemConfiguration> monitor, IOptions<SgSystemConfiguration> options, IConfiguration configuration)
    {
      _monitor = monitor;
      _configuration = configuration;
      _config = options.Value;

      _config = monitor.Get(Options.DefaultName);
    }
    
    [HttpGet]
    public IActionResult Index()
    {
      return Json(_config);
    }
    
    [HttpGet("{collection}")]
    public IActionResult Foo([FromRoute] string collection)
    {
      NotifyConfigurationChanged.ConfigurationChanged(collection);
      return Ok("Changed");
    }
  }
}
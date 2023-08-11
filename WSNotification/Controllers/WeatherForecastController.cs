using Microsoft.AspNetCore.Mvc;

namespace WSNotification.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    [HttpGet]
    public IEnumerable<string> Get()
    {
        
        return new List<string>();
    }
}
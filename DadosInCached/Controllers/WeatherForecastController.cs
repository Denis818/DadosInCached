using DadosInCached.CustomAttribute;
using Microsoft.AspNetCore.Mvc;

namespace DadosInCached.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Cached(15)]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
          "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        [HttpPost(Name = "GetWeatherForecast")]
        public IActionResult Post()
        {
            return Ok(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]

            }).ToArray());
        }
    }
}
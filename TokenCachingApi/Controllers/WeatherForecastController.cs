using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TokenCachingApi.Framework;

namespace TokenCachingApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly EndpointSetting _options;
        private readonly IHttpService _httpService;

        public WeatherForecastController(IOptions<EndpointSetting> options, 
            IHttpService httpService, 
            ILogger<WeatherForecastController> logger)
        {
            _options = options.Value;
            _httpService = httpService;
            _logger = logger;

        }

        [HttpGet(Name = "WeatherForecast")]
        public IEnumerable<WeatherForecast>? Get()
        {
            var httpResponse = _httpService.GetFromExternalApi(_options.BaseUri + "/WeatherForecast");
            var reader =new StreamReader(httpResponse.Content.ReadAsStream());
            var result = JsonConvert.DeserializeObject<List<WeatherForecast>>(reader.ReadToEnd());
            return result;
        }
    }
}
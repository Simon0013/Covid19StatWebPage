using Covid19StatWebPage.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Web;
using System.Text;

namespace Covid19StatWebPage.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        static List<CountryClass>? _countries;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<IActionResult> GetCountry(string lat, string lng)
        {
            var request = new UriBuilder("http://api.geonames.org/countryCode");
            request.Query = "?lat=" + lat + "&lng=" + lng + "&username=simon0013";
            var response = await new HttpClient().GetAsync(request.ToString());
            string answer = await response.Content.ReadAsStringAsync();
            if (answer.StartsWith("ERR:"))
                return NotFound();
			string? countryName = null;
			if (_countries == null) { 
			    var countries = await new HttpClient().GetAsync("https://api.covid19api.com/countries");
                var countriesFromApi = await countries.Content.ReadFromJsonAsync<List<CountryClass>>();
                
			    foreach (var country in countriesFromApi)
                {
                    if (answer.Contains(country.ISO2)) {
                        countryName = country.Slug; break;
				    }
			    }

                _countries = countriesFromApi.ToList();
			}
            else
            {
				foreach (var country in _countries)
				{
					if (answer.Contains(country.ISO2))
					{
						countryName = country.Slug; break;
					}
				}
			}
			if (string.IsNullOrEmpty(countryName))
                return NotFound();
            return Ok(countryName);
		}

        public async Task<IActionResult> GetCovid19Info(string country, string date, string mode)
        {
            var request = new StringBuilder("https://api.covid19api.com/country/");
            request.Append(country + "/status/" + mode + "?from=");
            DateTime dateTime = DateTime.Parse(date);
			dateTime = dateTime.AddDays(1);
			request.Append(date + "&to=");
            request.Append(dateTime.ToString("yyyy-MM-dd"));
            var response = await new HttpClient().GetAsync(request.ToString());
            var result = await response.Content.ReadFromJsonAsync<List<ByCountry>>();
            int cases = result.First().Cases;

            string countryName = null;
			foreach (var _country in _countries)
			{
				if (country.Equals(_country.Slug))
				{
					countryName = _country.Country; break;
				}
			}

			var answerToUser = new StringBuilder(countryName);
			answerToUser.Append("\nCOVID-19 information to " + date + ":\n");
			answerToUser.Append(mode.Substring(0, 1).ToUpper() + mode.Substring(1));
            answerToUser.Append(": " + cases);
            			
			return Ok(answerToUser.ToString());
        }

	}
}
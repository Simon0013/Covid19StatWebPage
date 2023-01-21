namespace Covid19StatWebPage.Models
{
	public class ByCountry
	{
		public string? Country { get; set; }
		public string? CountryCode { get; set; }
		public string? Province { get; set; }
		public string? City { get; set; }
		public string? CityCode { get; set; }
		public string? Lat { get; set; }
		public string? Lon { get; set; }
		public int Cases { get; set; }
		public string? Status { get; set; }
		public string? Date { get; set; }
	}
}

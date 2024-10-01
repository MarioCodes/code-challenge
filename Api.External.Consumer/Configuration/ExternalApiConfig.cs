namespace Api.Core.Configuration
{
    public class ExternalApiConfig
    {
        public const string Section = "ExternalApiConfig";

        public string BaseUrl { get; set; }
        public string AvailabilityEndpoint { get; set; }
        public string TakeSlotEndpoint { get; set; }
        public int RetryTimespanInSeconds { get; set; }
        public int RetryAttempts { get; set; }
    }

}

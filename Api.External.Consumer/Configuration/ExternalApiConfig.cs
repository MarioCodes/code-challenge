namespace Api.Core.Configuration
{
    public class ExternalApiConfig
    {
        public const string Section = "ExternalApiConfig";

        // TODO: use interface instead of virtual?
        public virtual string BaseUrl { get; set; }
        public virtual string AvailabilityEndpoint { get; set; }
        public string TakeSlotEndpoint { get; set; }
        public int RetryTimespanInSeconds { get; set; }
        public int RetryAttempts { get; set; }
    }

}

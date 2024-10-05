namespace Api.Core.Configuration
{
    public class ExternalApiConfig
    {
        public const string Section = "ExternalApiConfig";

        // TODO: use interface instead of virtual?
        public virtual string BaseUrl { get; set; }
        public virtual string AvailabilityEndpoint { get; set; }
        public string TakeSlotEndpoint { get; set; }
        public virtual int RetryTimespanInSeconds { get; set; }
        public virtual int RetryAttempts { get; set; }
    }

}

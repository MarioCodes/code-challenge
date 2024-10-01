namespace Api.Core.Configuration
{
    public class UserConfig
    {
        public const string Section = "UserConfig";

        public string User { get; set; }
        public string Password { get; set; }
        public string BaseUrl { get; set; }
    }
}

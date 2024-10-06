namespace Api.Core.Configuration
{
    public class AuthConfig
    {
        public const string Section = "AuthConfig";

        // TODO: I need to solve this virtual thing somehow -> interfaces instead? 
        public virtual string User { get; set; }
        public virtual string Password { get; set; }
    }

}

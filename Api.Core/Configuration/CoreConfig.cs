namespace Api.Core.Configuration
{
    public class CoreConfig
    {
        public const string Section = "CoreConfig";
        public virtual string InputDateFormat { get; set; }
        public virtual ErrorMessages ErrorMessages { get; set; }
    }

    public class ErrorMessages
    {
        public virtual string InputDateSetInPast { get; set; }
        public virtual string InputDateNotMonday { get; set; }
        public virtual string InputDateWrongFormat { get; set; }
        public virtual string InputDateGeneralError { get; set; }
    }
}

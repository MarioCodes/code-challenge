namespace Api.External.Consumer.Model
{
    public class TakeSlotRequest
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Comments { get; set; }
        public Patient Patient { get; set; }
    }

    public class Patient
    {
        public string Name { get; set; }
        public string SecondName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
    }
}

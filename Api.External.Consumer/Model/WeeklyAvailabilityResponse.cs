namespace Api.External.Consumer.Model
{
    public class WeeklyAvailabilityResponse
    {
        public Facility Facility { get; set; }
        public int SlotDurationMinutes { get; set; }
        public Day? Monday { get; set; }
        public Day? Tuesday { get; set; }
        public Day? Wednesday { get; set; }
        public Day? Thursday { get; set; }
        public Day? Friday { get; set; }
        public Day? Saturday { get; set; }
        public Day? Sunday { get; set; }
    }

    public class BusySlot
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
    }

    public class Facility
    {
        public string FacilityId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class Day
    {
        public WorkPeriod WorkPeriod { get; set; }
        public List<BusySlot> BusySlots { get; set; }
    }

    public class WorkPeriod
    {
        public int StartHour { get; set; }
        public int EndHour { get; set; }
        public int LunchStartHour { get; set; }
        public int LunchEndHour { get; set; }
    }


}

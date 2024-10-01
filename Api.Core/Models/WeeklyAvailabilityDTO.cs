using Api.External.Consumer.Model;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Api.Core.Models
{
    public class WeekAvailabilityDTO
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public FacilityDTO Facility { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DayDTO? Monday { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DayDTO? Tuesday { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DayDTO? Wednesday { get;set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DayDTO? Thursday { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DayDTO? Friday { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DayDTO? Saturday { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DayDTO? Sunday { get; set; }
    }

    public class FacilityDTO
    {
        public string Name { get; set; }
        public string Address { get; set; }
    }

    public class DayDTO
    {
        public List<AvailableSlotDTO> AvailableSlots { get; set; } = new List<AvailableSlotDTO>();
    }

    public class AvailableSlotDTO
    {
        public DateTime StartTime { get; set; }
    }
}

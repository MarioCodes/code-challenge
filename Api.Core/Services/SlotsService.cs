using Api.Core.Models;
using Api.Core.Services.interfaces;
using Api.External.Consumer.Model;
using Api.External.Consumer.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Core.Services
{
    public class SlotsService(IExternalApiService _externalApiService) : ISlotsService
    {
        public async Task<WeekAvailabilityDTO> GetWeekFreeSlotsAsync(DateOnly date)
        {
            var externalWeekData = await _externalApiService.GetWeeklyAvailabilityAsync(date);
            var weekAvailability = await GetWeekPlanning(date, externalWeekData);
            weekAvailability.Facility = await GetFacilityData(externalWeekData);
            return weekAvailability;
        }

        public async Task<string> ReserveSlotAsync(ReserveSlotDTO request)
        {
            // I do this mapping to decouple input data from data I send to the external API as it may change over time and we may need to adapt to it
            var mappedReserveSlot = await MapReserveSlot(request);
            return await _externalApiService.ReserveSlotAsync(mappedReserveSlot);
        }

        private async Task<ReserveSlotExternalRequest> MapReserveSlot(ReserveSlotDTO dto)
        {
            return new ReserveSlotExternalRequest
            {
                FacilityId = dto.FacilityId,
                Comments = dto.Comments,
                Start = dto.Start,
                End = dto.End,
                Patient = new Patient
                {
                    Email = dto.Patient.Email,
                    Name = dto.Patient.Name,
                    SecondName = dto.Patient.SecondName,
                    Phone = dto.Patient.Phone,
                }
            };
        }

        private async Task<FacilityDTO> GetFacilityData(WeeklyAvailabilityResponse externalData)
        {
            var facility = externalData.Facility;
            return new FacilityDTO
            {
                Name = facility?.Name ?? "",
                Address = facility?.Address ?? ""
            };
        }

        private async Task<WeekAvailabilityDTO> GetWeekPlanning(DateOnly date, WeeklyAvailabilityResponse externalWeekData) 
        {
            var weeklyPlanning = new WeekAvailabilityDTO();
            int duration = externalWeekData.SlotDurationMinutes;
            
            weeklyPlanning.Monday = await GetDayPlanning(date.AddDays(0), externalWeekData.Monday, duration);
            weeklyPlanning.Tuesday = await GetDayPlanning(date.AddDays(1), externalWeekData.Tuesday, duration);
            weeklyPlanning.Wednesday = await GetDayPlanning(date.AddDays(2), externalWeekData.Wednesday, duration);
            weeklyPlanning.Thursday = await GetDayPlanning(date.AddDays(3), externalWeekData.Thursday, duration);
            weeklyPlanning.Friday = await GetDayPlanning(date.AddDays(4), externalWeekData.Friday, duration);
            weeklyPlanning.Saturday = await GetDayPlanning(date.AddDays(5), externalWeekData.Saturday, duration);
            weeklyPlanning.Sunday = await GetDayPlanning(date.AddDays(6), externalWeekData.Sunday, duration);

            return weeklyPlanning;
        }

        private async Task<DayDTO?> GetDayPlanning(DateOnly date, Day? daySchedule, int slotDuration)
        {
            DayDTO day = new DayDTO();

            var daySlots = await GetDaySlots(date.AddDays(0), daySchedule, slotDuration);
            if(daySlots.Any())
            {
                day.AvailableSlots = daySlots;
                return day;
            }

            // if I didn't receive data for a given day this may return null (Task) so the serializer automatically hides all nulls on response (see WeekAvailabilityDTO [JsonIgnore] tag)
            return null;
        }

        private async Task<List<AvailableSlotDTO>> GetDaySlots(DateOnly inputDay, Day? daySchedule, int slotDuration)
        {
            if (daySchedule is null)
                return new List<AvailableSlotDTO>();

            List<AvailableSlotDTO> availableSlots = [];
            DateTime workshiftStart = await ConvertToDateTime(inputDay, daySchedule.WorkPeriod.StartHour);
            DateTime workshiftEnd = await ConvertToDateTime(inputDay, daySchedule.WorkPeriod.EndHour);

            DateTime lunchStart = await ConvertToDateTime(inputDay, daySchedule.WorkPeriod.LunchStartHour);
            DateTime lunchEnd = await ConvertToDateTime(inputDay, daySchedule.WorkPeriod.LunchEndHour);

            DateTime slotStart = workshiftStart;
            while (slotStart < workshiftEnd)
            {
                DateTime slotEnd = slotStart.AddMinutes(slotDuration);

                if (await ItsLunchtime(lunchStart, lunchEnd, slotStart))
                {
                    // for cases where lunch duration is > 1 hour
                    int lunchTimeDuration =  lunchEnd.Hour - lunchStart.Hour;
                    slotStart = slotStart.AddHours(lunchTimeDuration);
                    // I also reset minutes for cases where we have a slot duration such as 45, so slots start again right after lunchtime
                    slotStart = slotStart.AddMinutes(-slotStart.Minute);
                    continue;
                }

                if (await IsSlotFree(daySchedule, slotStart, slotEnd))
                {
                    availableSlots.Add(new AvailableSlotDTO { StartTime = slotStart });
                }

                slotStart = slotEnd;
            }

            return availableSlots;
        }

        private async Task<bool> IsSlotFree(Day daySchedule, DateTime slotStart, DateTime slotEnd)
        {
            bool isSlotBusy = daySchedule.BusySlots?.Any(busy => slotStart < busy.End && slotEnd > busy.Start) ?? false;
            return !isSlotBusy;
        }

        private async Task<bool> ItsLunchtime(DateTime lunchStart, DateTime lunchEnd, DateTime slotStart)
        {
            return slotStart.Hour >= lunchStart.Hour && slotStart.Hour < lunchEnd.Hour;
        }

        private async Task<DateTime> ConvertToDateTime(DateOnly dateOnly, int hour)
        {
            var time = new TimeOnly(hour, 0);
            return dateOnly.ToDateTime(time);
        }
    }
}

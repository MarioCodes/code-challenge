using Api.External.Consumer.Model;

namespace Api.External.Consumer.Services.Interfaces
{
    public interface IExternalApiService
    {
        Task<WeeklyAvailabilityResponse> GetWeeklyAvailabilityAsync(DateOnly date);
        Task TakeSlotAsync(TakeSlotRequest slotRequest);
    }
}

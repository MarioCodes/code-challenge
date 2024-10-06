using Api.External.Consumer.Model;

namespace Api.External.Consumer.Services.Interfaces
{
    public interface IExternalApiService
    {
        /// <summary>
        /// Retrieves weekly availability slots from the external API for the specified date.
        /// </summary>
        /// <param name="date">The date to use to retrieve all available slots.</param>
        /// <returns>A <see cref="WeeklyAvailabilityResponse"/> which has the available slots for the week.</returns>
        Task<WeeklyAvailabilityResponse> GetWeeklyAvailabilityAsync(DateOnly date);

        /// <summary>
        /// Sends a request to reserve a slot via the external API.
        /// </summary>
        /// <param name="slotRequest">The <see cref="ReserveSlotExternalRequest"/> object containing the details of the slot to be reserved.</param>
        /// <returns>A string which has the response from the external API.</returns>
        Task<string> ReserveSlotAsync(ReserveSlotExternalRequest slotRequest);
    }
}

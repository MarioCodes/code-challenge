using Api.Core.Models;
using Api.External.Consumer.Model;
using System;
using System.Threading.Tasks;

namespace Api.Core.Services.interfaces
{
    public interface ISlotsService
    {
        Task<WeekAvailabilityDTO> GetWeekFreeSlotsAsync(DateOnly date);
        Task<string> ReserveSlotAsync(ReserveSlotDTO request);

    }
}

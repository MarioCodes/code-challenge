using Api.Core.Configuration;
using Api.External.Consumer.Common.Interfaces;
using Api.External.Consumer.Model;
using Api.External.Consumer.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace Api.External.Consumer.Services
{
    public class ExternalApiService(HttpClient _httpClient, 
        IHttpService _httpService, 
        IOptions<ExternalApiConfig> _options) : IExternalApiService
    {
        private ExternalApiConfig _config => _options.Value;

        public async Task<WeeklyAvailabilityResponse> GetWeeklyAvailabilityAsync(DateOnly date)
        {
            // TODO: set format at config level
            string parsedDate = date.ToString("yyyyMMdd");
            string url = await BuildUrl(parsedDate);
            
            HttpRequestMessage request = _httpService.SetUpGet(url);
            string response = await _httpService.HttpCallAsync(_httpClient, request);
            return JsonConvert.DeserializeObject<WeeklyAvailabilityResponse>(response);
        }

        private async Task<string> BuildUrl(string date)
        {
            StringBuilder fullUrl = new StringBuilder(_config.BaseUrl);
            fullUrl = fullUrl.Append(_config.AvailabilityEndpoint);
            fullUrl = fullUrl.Append(date);
            return fullUrl.ToString();
        }

        public async Task TakeSlotAsync(TakeSlotRequest slotRequest)
        {
            throw new NotImplementedException();
        }
    }
}

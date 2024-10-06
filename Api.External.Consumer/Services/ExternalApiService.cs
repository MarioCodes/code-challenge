﻿using Api.Core.Configuration;
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
            string parsedDate = date.ToString(_config.ExternalApiDateFormat);
            string endpoint = _config.AvailabilityEndpoint;
            string url = await BuildUrl(endpoint, parsedDate);
            
            string response = await _httpService.HttpCallAsync(_httpClient, () => _httpService.SetUpGet(url));
            // TODO: check this may be null here, maybe return WeeklyAvailabilityResponse? check if this changes tests or return values
            return JsonConvert.DeserializeObject<WeeklyAvailabilityResponse>(response);
        }

        private async Task<string> BuildUrl(string endpoint, string date = "")
        {
            StringBuilder fullUrl = new StringBuilder(_config.BaseUrl);
            fullUrl = fullUrl.Append(endpoint);

            if(date is not "")
                fullUrl = fullUrl.Append(date);

            return fullUrl.ToString();
        }

        public async Task<string> ReserveSlotAsync(ReserveSlotExternalRequest slotRequest)
        {
            string endpoint = _config.TakeSlotEndpoint;
            string url = await BuildUrl(endpoint);
            string response = await _httpService.HttpCallAsync(_httpClient, () => _httpService.SetUpPost(url, slotRequest));
            return response;
        }
    }
}

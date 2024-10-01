namespace Api.External.Consumer.Common.Interfaces
{
    public interface IHttpService
    {
        Task<string> HttpCallAsync(HttpClient client, HttpRequestMessage request);
        HttpRequestMessage SetUpGet(string url);
        HttpRequestMessage SetUpPost(string url, object payload);
    }
}

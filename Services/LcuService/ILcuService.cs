namespace RiotAccountManager.MAUI.Services.LcuService;

public interface ILcuService
{
    Task<HttpResponseMessage> SendRequestAsync(
        HttpMethod method,
        string endpoint,
        string? data = null
    );
    Task<T?> GetAsync<T>(string endpoint);
    Task<T?> PostAsync<T>(string endpoint, object data);
}

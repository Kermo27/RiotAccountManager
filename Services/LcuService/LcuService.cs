using System.Net.Http.Json;
using System.Text;
using BlossomiShymae.GrrrLCU;

namespace RiotAccountManager.MAUI.Services.LcuService;

public class LcuService : ILcuService
{
    private HttpClient _client;

    public LcuService()
    {
        _client = Connector.GetLcuHttpClientInstance();
    }

    public async Task<HttpResponseMessage> SendRequestAsync(
        HttpMethod method,
        string endpoint,
        string? data = null
    )
    {
        var request = new HttpRequestMessage(method, endpoint);

        if (data != null && method != HttpMethod.Get)
        {
            request.Content = new StringContent(data, Encoding.UTF8, "application/json");
        }
        else if (data != null && method == HttpMethod.Get)
        {
            request.RequestUri = new Uri($"{endpoint}?{data}");
        }

        return await _client.SendAsync(request);
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        try
        {
            return await _client.GetFromJsonAsync<T>(endpoint);
        }
        catch (Exception)
        {
            return default;
        }
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        var response = await _client.PostAsJsonAsync(endpoint, data);

        return await response.Content.ReadFromJsonAsync<T>();
    }
}

namespace Mango.Web.Services;

using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using IServices;
using Models;

public class BaseService : IBaseService
{
    public ResponseDto responseModel { get; set; }
    public IHttpClientFactory ClientFactory { get; set; }

    public BaseService(IHttpClientFactory clientFactory)
    {
        this.responseModel = new ResponseDto();
        this.ClientFactory = clientFactory;
    }

    public async Task<T> SendAsync<T>(ApiRequest apiRequest)
    {
        try
        {
            var client = ClientFactory.CreateClient("MangoAPI");
            var message = new HttpRequestMessage();
            message.Headers.Add("Accept", "application/json");
            message.RequestUri = new Uri(apiRequest.Url);
            client.DefaultRequestHeaders.Clear();
            if (apiRequest.Data != null)
            {
                message.Content = new StringContent(JsonSerializer.Serialize(apiRequest.Data), 
                    Encoding.UTF8, "application/json");
            }

            if (!string.IsNullOrEmpty(apiRequest.AccessToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", 
                    apiRequest.AccessToken);
            }

            message.Method = apiRequest.ApiType switch
            {
                SD.ApiType.POST => HttpMethod.Post,
                SD.ApiType.PUT => HttpMethod.Put,
                SD.ApiType.DELETE => HttpMethod.Delete,
                SD.ApiType.GET => HttpMethod.Get,
                _ => HttpMethod.Get
            };

            var apiResponse = await client.SendAsync(message);
            var apiContent = await apiResponse.Content.ReadAsStringAsync();
            var apiResponseDto = JsonSerializer.Deserialize<T>(apiContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return apiResponseDto;
        }
        catch (Exception e)
        {
            var dto = new ResponseDto()
            {
                DisplayMessage = "Error",
                ErrorMessages = new List<string> { e.Message },
                IsSuccess = false
            };
            var res = JsonSerializer.Serialize(dto);
            var apiResponseDto = JsonSerializer.Deserialize<T>(res);
            return apiResponseDto;
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(true);
    }
}
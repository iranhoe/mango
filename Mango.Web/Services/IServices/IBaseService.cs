namespace Mango.Web.Services.IServices;

using Models;

public interface IBaseService : IDisposable
{
    ResponseDto responseModel { get; set; }
    Task<T> SendAsync<T>(ApiRequest apiRequest);
}
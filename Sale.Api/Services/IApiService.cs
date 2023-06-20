using Sale.Shared.Response;

namespace Sale.Api.Services
{
    public interface IApiService
    {
        Task<Response> GetListAsync<T>(string servicePrefix, string controller);
    }
}

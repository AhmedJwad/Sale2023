using Sale.Shared.Response;

namespace Sale.Api.Helpers
{
    public interface IOrdersHelper
    {
        Task<Response> ProcessOrderAsync(string email, string remarks);

    }
}

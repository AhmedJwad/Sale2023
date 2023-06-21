using Sale.Shared.Response;

namespace Sale.Api.Helpers
{
    public interface IMailHelper
    {
        Response SendMail(string toName, string toEmail, string subject, string body);

    }
}

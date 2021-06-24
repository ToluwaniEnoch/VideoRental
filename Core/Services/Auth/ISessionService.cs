using Api.Models.Internals;

namespace Api.Services.Auth
{
    public interface ISessionService
    {
        Session? GetSession();

        bool HasPermission(string permission);
    }
}
using Api.Data.Entities.Account;
using MediatR;

namespace Api.Services.Auth.Notifications
{
    public class PersonResetPasswordNotification : INotification
    {
        public Persona? Person { get; set; }
    }
}
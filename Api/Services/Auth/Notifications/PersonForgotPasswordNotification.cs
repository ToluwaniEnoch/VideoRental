using Api.Data.Entities.Account;
using MediatR;

namespace Api.Services.Auth.Notifications
{
    public class PersonForgotPasswordNotification : INotification
    {
        public Persona? Person { get; set; }
        public string? Otp { get; set; }
    }
}
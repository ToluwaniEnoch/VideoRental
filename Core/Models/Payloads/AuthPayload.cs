using Api.Models.Validation;
using System.ComponentModel.DataAnnotations;

namespace Api.Models.Payloads
{
    [AtLeastOneProperty("Email", ErrorMessage = "Email is required")]
    public record LoginPayload([EmailAddress] string Email, [Required(ErrorMessage = "Password Field Is Required")] string Password);

    public record ChangePasswordPayload([Required(ErrorMessage = "Old Password Field Is Required")] string OldPassword, [Required(ErrorMessage = "Password Field Is Required"), MinLength(6, ErrorMessage = "Passwords Must Be At Least 6 Characters Long")] string Password)
    {
        [Required(ErrorMessage = "Confirm Password Field Is Required"), Compare(nameof(Password), ErrorMessage = "Password Fields Do Not Match")]
        public string? ConfirmPassword { get; init; }
    }

    public record ForgotPasswordPayload([Required(ErrorMessage = "Email Field Is Required"), EmailAddress] string Email);

    public record ResetPasswordPayload(
        [Required(ErrorMessage = "Email Field Is Required"),EmailAddress] string Email,
        [Required(ErrorMessage = "Token Field Is Required")] string Token,
        [Required, MinLength(6)] string NewPassword)
    {
        [Required, Compare(nameof(NewPassword))]
        public string? ConfirmPassword { get; init; }
    }
}
using CodeHorizon.Blazor.Models.Auth;
using FluentValidation;

namespace CodeHorizon.Blazor.Helpers.Validators;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
    }
}

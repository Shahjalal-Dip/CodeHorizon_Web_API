using CodeHorizon.Blazor.Models.Users;
using FluentValidation;

namespace CodeHorizon.Blazor.Helpers.Validators;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.Bio).MaximumLength(500);
        RuleFor(x => x.ProfilePictureUrl).MaximumLength(200);
        RuleFor(x => x.FullName).MaximumLength(100);
    }
}

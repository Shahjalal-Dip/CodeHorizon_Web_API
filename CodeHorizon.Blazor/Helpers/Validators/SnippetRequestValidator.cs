using CodeHorizon.Blazor.Models.Snippets;
using FluentValidation;

namespace CodeHorizon.Blazor.Helpers.Validators;

public class SnippetRequestValidator : AbstractValidator<SnippetRequest>
{
    public SnippetRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(3).MaximumLength(200);
        RuleFor(x => x.Content).NotEmpty().MinimumLength(10);
        RuleFor(x => x.Description).MaximumLength(500);
        RuleFor(x => x.Language).NotEmpty();
        RuleFor(x => x.Tags).Must(t => t.Count <= 10).WithMessage("Maximum 10 tags allowed");
    }
}

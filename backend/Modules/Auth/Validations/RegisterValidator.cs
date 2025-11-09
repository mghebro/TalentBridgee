using FluentValidation;
using TalentBridge.Modules.Auth.DTOs.Requests;

namespace TalentBridge.Modules.Auth.Validations;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(x => x.FirstName)
              .NotEmpty().WithMessage("First name cannot be empty")
              .Length(2, 50).WithMessage("First name must be between 2 and 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name cannot be empty")
            .Length(2, 50).WithMessage("Last name must be between 2 and 50 characters");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email cannot be empty")
            .EmailAddress().WithMessage("Invalid email address");
    }
}
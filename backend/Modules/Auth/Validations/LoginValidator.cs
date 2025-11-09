using FluentValidation;
using TalentBridge.Modules.Auth.DTOs.Requests;

namespace TalentBridge.Modules.Auth.Validations;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email cannot be empty")
            .EmailAddress().WithMessage("Invalid email address");
    }
}

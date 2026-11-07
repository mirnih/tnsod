using ConsoleApp1.Protos;
using FluentValidation;

namespace ConsoleApp1.Application.Validators;

public class UpdateUserValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name cannot be empty");
        RuleFor(x => x.Surname).NotEmpty().WithMessage("Surname cannot be empty");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Password cannot be empty").Length(6, 30).WithMessage("Password must be between 6 and 30 characters");
        RuleFor(x => x.Age).NotEmpty().WithMessage("Age cannot be empty");
    }
}
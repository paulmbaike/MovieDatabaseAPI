using FluentValidation;
using MovieDatabaseAPI.Core.DTOs;

namespace MovieDatabaseAPI.API.Validators;

public class CreateActorValidator : AbstractValidator<CreateActorDto>
{
    public CreateActorValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100)
            .WithMessage("Name is required and must not exceed 100 characters");

        RuleFor(x => x.Bio).MaximumLength(2000)
            .WithMessage("Bio must not exceed 2000 characters");

        RuleFor(x => x.DateOfBirth).LessThanOrEqualTo(DateTime.Now)
            .WithMessage("Date of birth cannot be in the future");
    }
}

public class UpdateActorValidator : AbstractValidator<UpdateActorDto>
{
    public UpdateActorValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100)
            .WithMessage("Name is required and must not exceed 100 characters");

        RuleFor(x => x.Bio).MaximumLength(2000)
            .WithMessage("Bio must not exceed 2000 characters");

        RuleFor(x => x.DateOfBirth).LessThanOrEqualTo(DateTime.Now)
            .WithMessage("Date of birth cannot be in the future");
    }
}
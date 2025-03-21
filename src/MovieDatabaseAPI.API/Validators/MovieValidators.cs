using FluentValidation;
using MovieDatabaseAPI.Core.DTOs;

namespace MovieDatabaseAPI.API.Validators;

public class CreateMovieValidator : AbstractValidator<CreateMovieDto>
{
    public CreateMovieValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200)
            .WithMessage("Title is required and must not exceed 200 characters");

        RuleFor(x => x.ReleaseYear).InclusiveBetween(1888, DateTime.Now.Year + 5)
            .WithMessage($"Release year must be between 1888 and {DateTime.Now.Year + 5}");

        RuleFor(x => x.Plot).MaximumLength(2000)
            .WithMessage("Plot must not exceed 2000 characters");

        RuleFor(x => x.RuntimeMinutes).GreaterThan(0)
            .WithMessage("Runtime must be greater than 0 minutes");

        RuleFor(x => x.PosterUrl).MaximumLength(500)
            .WithMessage("Poster URL must not exceed 500 characters");
    }
}

public class UpdateMovieValidator : AbstractValidator<UpdateMovieDto>
{
    public UpdateMovieValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200)
            .WithMessage("Title is required and must not exceed 200 characters");

        RuleFor(x => x.ReleaseYear).InclusiveBetween(1888, DateTime.Now.Year + 5)
            .WithMessage($"Release year must be between 1888 and {DateTime.Now.Year + 5}");

        RuleFor(x => x.Plot).MaximumLength(2000)
            .WithMessage("Plot must not exceed 2000 characters");

        RuleFor(x => x.RuntimeMinutes).GreaterThan(0)
            .WithMessage("Runtime must be greater than 0 minutes");

        RuleFor(x => x.PosterUrl).MaximumLength(500)
            .WithMessage("Poster URL must not exceed 500 characters");
    }
}
using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Validators;

public class MoviesValidator : AbstractValidator<Movie>
{
    private readonly IMovieRepository _movieRepository;
    
    public MoviesValidator(IMovieRepository movieRepository)
    {
        _movieRepository = movieRepository;
        
        RuleFor(m => m.Id)
            .NotEmpty();
        
        RuleFor(m => m.Title)
            .NotEmpty();

        RuleFor(m => m.Genres)
            .NotEmpty();
        
        RuleFor(m => m.YearOfRelease)
            .LessThanOrEqualTo(DateTime.Now.Year);
        
        RuleFor(m => m.Slug)
            .MustAsync(ValidateSlug)
            .WithMessage("The movie already exists in the system");
    }
    
    private async Task<bool> ValidateSlug(Movie movie, string slug, CancellationToken token = default)
    {
        var existingMovie = await _movieRepository.GetBySlugAsync(slug);

        if (existingMovie is not null)
            return existingMovie.Id == movie.Id;
        
        return existingMovie is null;
    }
}

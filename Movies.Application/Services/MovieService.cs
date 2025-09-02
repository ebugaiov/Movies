using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IValidator<Movie> _validator;
    private readonly IRatingRepository _ratingRepository;
    private readonly IValidator<GetAllMoviesOptions> _optionsValidator;

    public MovieService(
        IMovieRepository movieRepository, 
        IValidator<Movie> validator,
        IRatingRepository ratingRepository, 
        IValidator<GetAllMoviesOptions> optionsValidator)
    {
        _movieRepository = movieRepository;
        _validator = validator;
        _ratingRepository = ratingRepository;
        _optionsValidator = optionsValidator;
    }
    
    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        await _validator.ValidateAndThrowAsync(movie, cancellationToken: token);
        return await _movieRepository.CreateAsync(movie, token);
    }

    public async Task<Movie?> UpdateAsync(Movie movie,
        Guid? userId = default,
        CancellationToken token = default)
    {
        await _validator.ValidateAndThrowAsync(movie, cancellationToken: token);
        var movieExists = _movieRepository.ExistsByIdAsync(movie.Id, token);
        if (!movieExists.Result)
            return null;
        
        await _movieRepository.UpdateAsync(movie, token);

        if (!userId.HasValue)
        {
            var rating = await _ratingRepository.GetRatingAsync(movie.Id, token);
            movie.Rating = rating;
            return movie;
        }
        
        var ratings = _ratingRepository.GetRatingAsync(movie.Id, userId.Value, token);
        movie.Rating = ratings.Result.Rating;
        movie.UserRating = ratings.Result.UserRating;
        
        return movie;
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        return _movieRepository.DeleteByIdAsync(id, token);
    }

    public Task<Movie?> GetByIdAsync(
        Guid id, 
        Guid? userId = default,
        CancellationToken token = default)
    {
        return _movieRepository.GetByIdAsync(id, userId, token);
    }

    public Task<Movie?> GetBySlugAsync(
        string slug, 
        Guid? userId = default,
        CancellationToken token = default)
    {
        return _movieRepository.GetBySlugAsync(slug, userId, token);
    }

    public async Task<IEnumerable<Movie>> GetAllAsync(GetAllMoviesOptions options,
        CancellationToken token = default)
    {
        await _optionsValidator.ValidateAndThrowAsync(options, cancellationToken: token);
        
        return await _movieRepository.GetAllAsync(options, token);
    }

    public Task<int> GetCountAsync(string? title, int? yearOfRelese, CancellationToken token = default)
    {
        return _movieRepository.GetCountAsync(title, yearOfRelese, token);
    }
}
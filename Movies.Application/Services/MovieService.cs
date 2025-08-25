using FluentValidation;
using Movies.Application.Models;
using Movies.Application.Repositories;

namespace Movies.Application.Services;

public class MovieService : IMovieService
{
    private readonly IMovieRepository _movieRepository;
    private readonly IValidator<Movie> _validator;

    public MovieService(IMovieRepository movieRepository, IValidator<Movie> validator)
    {
        _movieRepository = movieRepository;
        _validator = validator;
    }
    
    public async Task<bool> CreateAsync(Movie movie, CancellationToken token = default)
    {
        await _validator.ValidateAndThrowAsync(movie, cancellationToken: token);
        return await _movieRepository.CreateAsync(movie, token);
    }

    public async Task<Movie?> UpdateAsync(Movie movie, CancellationToken token = default)
    {
        await _validator.ValidateAndThrowAsync(movie, cancellationToken: token);
        var movieExists = _movieRepository.ExistsByIdAsync(movie.Id, token);
        if (!movieExists.Result)
            return null;
        
        await _movieRepository.UpdateAsync(movie, token);
        
        return movie;
    }

    public Task<bool> DeleteByIdAsync(Guid id, CancellationToken token = default)
    {
        return _movieRepository.DeleteByIdAsync(id, token);
    }

    public Task<Movie?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return _movieRepository.GetByIdAsync(id, token);
    }

    public Task<Movie?> GetBySlugAsync(string slug, CancellationToken token = default)
    {
        return _movieRepository.GetBySlugAsync(slug, token);
    }

    public Task<IEnumerable<Movie>> GetAllAsync(CancellationToken token = default)
    {
        return _movieRepository.GetAllAsync(token);
    }
}
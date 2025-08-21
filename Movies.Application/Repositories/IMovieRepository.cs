using Movies.Application.Models;

namespace Movies.Application.Repositories;

public interface IMovieRepository
{
    Task<bool> CreateAsync(Movie movie);
    
    Task<bool> UpdateAsync(Movie movie);
    
    Task<bool> DeleteByIdAsync(Guid id);
    
    Task<Movie?> GetByIdAsync(Guid id);
    
    Task<IEnumerable<Movie>> GetAllAsync();
}

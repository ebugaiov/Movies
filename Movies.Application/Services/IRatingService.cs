namespace Movies.Application.Services;

public interface IRatingService
{
    Task<bool> RateMovieAsync(Guid movieId, int rating, Guid userId, CancellationToken token = default);

    Task<bool> DeleteRating(Guid movieId, Guid userId, CancellationToken token = default);
}
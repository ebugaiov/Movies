using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Movies.Api.Mapping;
using Movies.Application.Repositories;
using Movies.Application.Services;
using Movies.Contracts.Requests;

namespace Movies.Api.Controllers;

[ApiController] 
public class MoviesController : ControllerBase
    {
        private readonly IMovieService _movieService;
        
        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [Authorize(AuthConstants.AdminUserPolicyName)]
        [HttpPost(ApiEndpoints.Movies.Create)]
        public async Task<IActionResult> CreateMovie(CreateMovieRequest request, 
            CancellationToken token)
        {
            var movie = request.MapToMovie();
            await _movieService.CreateAsync(movie, token);
            return CreatedAtAction(nameof(Get), new { idOrSlug = movie.Id }, movie);
        }

        [HttpGet(ApiEndpoints.Movies.Get)]
        public async Task<IActionResult> Get(string idOrSlug, CancellationToken token)
        {
            var movie = Guid.TryParse(idOrSlug, out var id)
                ? await _movieService.GetByIdAsync(id, token)
                : await _movieService.GetBySlugAsync(idOrSlug, token);
            if (movie is null) return NotFound();
            var response = movie.MapToResponse();
            return Ok(response);
        }

        [HttpGet(ApiEndpoints.Movies.GetAll)]
        public async Task<IActionResult> GetAllMovies(CancellationToken token)
        {
            var movies = await _movieService.GetAllAsync(token);
            var response = movies.MapToResponse();
            return Ok(response);
        }

        [Authorize(AuthConstants.AdminUserPolicyName)]
        [HttpPut(ApiEndpoints.Movies.Update)]
        public async Task<IActionResult> Update(Guid id, UpdateMovieRequest request, 
            CancellationToken token)
        {
            var movie = request.MapToMovie(id);
            var updatedMovie = await _movieService.UpdateAsync(movie, token);
            if (updatedMovie is null) return NotFound();
            var response = movie.MapToResponse();
            return Ok(response);
        }

        [Authorize(AuthConstants.AdminUserPolicyName)]
        [HttpDelete(ApiEndpoints.Movies.Delete)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken token)
        {
            var deleted = await _movieService.DeleteByIdAsync(id, token);
            if (!deleted) return NotFound();
            return Ok();
        }
    }

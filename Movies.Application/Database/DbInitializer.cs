using Dapper;

namespace Movies.Application.Database;

public class DbInitializer
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    
    public DbInitializer(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task InitializeAsync()
    {
        using var connection = await _dbConnectionFactory.CreateConnectionAsync();
        
        // Create movie table
        await connection.ExecuteAsync("""
            create table if not exists movies (
                id uuid primary key,
                title text not null,
                slug text not null,
                yearofrelease integer not null
            );
        """);
        
        // Create genre table
        await connection.ExecuteAsync("""
            create table if not exists genres (
                movieId UUID references movies(Id),
                name text not null
            );
        """);
        
        // Create ratings table
        await connection.ExecuteAsync("""
            create table if not exists ratings (
                userid uuid,
                movieid uuid references movies(id),
                rating integer not null,
                primary key (userid, movieid)
            );
        """);
        
        // Create unique index on slug
        await connection.ExecuteAsync("""
            create unique index if not exists idx_movies_slug 
            on movies using btree(slug);
        """);
    }
}

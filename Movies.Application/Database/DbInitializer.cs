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
        await connection.ExecuteAsync("""
            create table if not exists movies (
                id uuid primary key,
                title text not null,
                slug text not null,
                yearofrelease integer not null
            );
        """);
        
        await connection.ExecuteAsync("""
            create unique index concurrently if not exists idx_movies_slug 
            on movies 
            using btree(slug);
        """);
    }
}

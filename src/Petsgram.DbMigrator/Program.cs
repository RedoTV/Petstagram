using Microsoft.EntityFrameworkCore;
using Petsgram.Infrastructure.DbContexts;

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DbConnection");

var optionsBuilder = new DbContextOptionsBuilder<PetsgramDbContext>();
optionsBuilder.UseSqlServer(connectionString);

using var context = new PetsgramDbContext(optionsBuilder.Options);

try
{
    Console.WriteLine("Checking database connection...");
    await context.Database.CanConnectAsync();
    Console.WriteLine("Database connection successful");

    Console.WriteLine("Applying database migrations...");
    await context.Database.MigrateAsync();
    Console.WriteLine("Migration complete");
}
catch (Microsoft.Data.SqlClient.SqlException sqlEx)
{
    Console.WriteLine($"Database error: {sqlEx.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
    Console.WriteLine($"Stack trace: {ex.StackTrace}");
}
finally
{
    Console.WriteLine("Migration process finished");
}
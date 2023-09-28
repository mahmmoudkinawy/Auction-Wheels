namespace Auctions.Api.Extensions;
public static class DatabaseInitializerExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(nameof(app));

        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuctionsDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            await dbContext.Database.MigrateAsync();
            await Seed.SeedAuctionsWithItemsAsync(dbContext);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Auctions.Api - An error occurred while applying pending migrations.");
        }

    }
}

namespace Search.Api.Database;
public static class DbInitializer
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(app));

        await DB.InitAsync(
            app.Configuration.GetValue<string>("MongoDb:DatabaseName"),
            MongoClientSettings
                .FromConnectionString(app.Configuration.GetValue<string>("MongoDb:ConnectionString")));

        await DB.Index<ItemModel>()
            .Key(i => i.Make, KeyType.Text)
            .Key(i => i.Color, KeyType.Text)
            .Key(i => i.Model, KeyType.Text)
            .CreateAsync();

        using var scope = app.Services.CreateScope();

        var auctionServiceHttpClient = scope.ServiceProvider.GetRequiredService<SyncAuctionService>();

        var items = await auctionServiceHttpClient.SyncItemsForSearchDb();

        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        logger.LogInformation($"{items.Count} - returned from the auctions api.");

        if (items.Any())
        {
            await DB.SaveAsync(items);
        }
    }
}

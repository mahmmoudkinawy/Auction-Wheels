namespace Search.Api.Services;
public sealed class SyncAuctionService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public SyncAuctionService(
        HttpClient httpClient,
        IConfiguration config)
    {
        _httpClient = httpClient ??
            throw new ArgumentNullException(nameof(httpClient));
        _config = config ??
            throw new ArgumentNullException(nameof(config));
    }

    public async Task<IReadOnlyList<ItemModel>> SyncItemsForSearchDb()
    {
        var lastUpdate = await DB.Find<ItemModel, string>()
            .Sort(_ => _.Descending(i => i.UpdatedAt))
            .Project(i => i.UpdatedAt.ToString())
            .ExecuteFirstAsync();

        return await _httpClient.GetFromJsonAsync<IReadOnlyList<ItemModel>>(
            $"{_config.GetValue<string>("AuctionServiceApiUrl")}/auctions?date={lastUpdate}");
    }
}

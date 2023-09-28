namespace Auctions.Api.Contracts.Requests;
public sealed class UpdateAuctionRequest
{
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public string? Color { get; set; }
    public int? Mileage { get; set; }
}

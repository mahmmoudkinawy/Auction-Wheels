namespace Auctions.Api.Entities;
public sealed class AuctionEntity
{
    public Guid Id { get; set; }
    public int? ReservePrice { get; set; }
    public string? Seller { get; set; }
    public string? Winner { get; set; }
    public int? SoldAmount { get; set; }
    public int? CurrentHightBid { get; set; }
    public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? AuctionEnd { get; set; }
    public StatusEnum? Status { get; set; } = StatusEnum.Live;

    public ItemEntity? Item { get; set; }
}

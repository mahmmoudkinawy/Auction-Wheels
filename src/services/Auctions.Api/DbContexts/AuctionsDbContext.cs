namespace Auctions.Api.DbContexts;
public sealed class AuctionsDbContext : DbContext
{
    public AuctionsDbContext(DbContextOptions<AuctionsDbContext> options) : base(options)
    { }

    public DbSet<AuctionEntity> Auctions { get; set; }
    public DbSet<ItemEntity> Items { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<AuctionEntity>()
            .HasOne(a => a.Item)
            .WithOne(i => i.Auction)
            .HasForeignKey<ItemEntity>(i => i.AuctionId)
            .IsRequired();

        base.OnModelCreating(builder);
    }
}

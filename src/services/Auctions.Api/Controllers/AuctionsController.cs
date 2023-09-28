namespace Auctions.Api.Controllers;

// I didn't implement any design patterns because the application primarily involves straightforward CRUD operations.

[Route("api/auctions")]
[ApiController]
public sealed class AuctionsController : ControllerBase
{
    private readonly AuctionsDbContext _context;
    private readonly IMapper _mapper;

    public AuctionsController(AuctionsDbContext context, IMapper mapper)
    {
        _context = context ??
            throw new ArgumentNullException(nameof(context));
        _mapper = mapper ??
            throw new ArgumentNullException(nameof(mapper));
    }

    [HttpGet]
    public async Task<IActionResult> GetAuctions(CancellationToken cancellationToken)
    {
        var auctions = await _context.Auctions
            .Include(a => a.Item)
            .OrderBy(a => a.Item.Make)
            .ToListAsync(cancellationToken);

        return Ok(_mapper.Map<IReadOnlyList<AuctionResponse>>(auctions));
    }

    [HttpGet("{auctionId}", Name = "GetAuctionById")]
    public async Task<IActionResult> GetAuctionById(
        [FromRoute] Guid auctionId,
        CancellationToken cancellationToken)
    {
        var auction = await _context.Auctions
            .Include(a => a.Item)
            .FirstOrDefaultAsync(a => a.Id == auctionId, cancellationToken: cancellationToken);

        if (auction is null)
        {
            return NotFound();
        }

        return Ok(_mapper.Map<AuctionResponse>(auction));
    }


    [HttpPost]
    public async Task<IActionResult> CreateAuction(
        [FromBody] CreateAuctionRequest request,
        CancellationToken cancellationToken)
    {
        var auction = _mapper.Map<AuctionEntity>(request);

        // TODO: add current user as seller

        auction.Seller = "test";

        _context.Auctions.Add(auction);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtRoute(
            nameof(GetAuctionById),
            new { auctionId = auction.Id },
            _mapper.Map<AuctionResponse>(auction));
    }

    [HttpPut("{auctionId}")]
    public async Task<IActionResult> UpdateAuction(
       [FromRoute] Guid auctionId,
       [FromBody] UpdateAuctionRequest request,
       CancellationToken cancellationToken)
    {
        var auction = await _context.Auctions
            .Include(a => a.Item)
            .FirstOrDefaultAsync(a => a.Id == auctionId, cancellationToken: cancellationToken);

        if (auction is null)
        {
            return NotFound();
        }

        // TODO: check seller == username

        auction.Item.Make = request.Make ?? auction.Item.Make;
        auction.Item.Model = request.Model ?? auction.Item.Model;
        auction.Item.Color = request.Color ?? auction.Item.Color;
        auction.Item.Mileage = request.Mileage ?? auction.Item.Mileage;
        auction.Item.Year = request.Year ?? auction.Item.Year;

        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    [HttpDelete("{auctionId}")]
    public async Task<IActionResult> DeleteAction(
       [FromRoute] Guid auctionId,
       CancellationToken cancellationToken)
    {
        var auction = await _context.Auctions
            .FindAsync(new object?[] { auctionId }, cancellationToken: cancellationToken);

        if (auction is null)
        {
            return NotFound();
        }

        _context.Auctions.Remove(auction);
        await _context.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

}

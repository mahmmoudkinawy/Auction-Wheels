namespace Search.Api.Controllers;

[Route("api/search")]
[ApiController]
public sealed class SearchController : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> SearchItems(
        [FromQuery] SearchParams searchParams,
        CancellationToken cancellationToken)
    {
        var query = DB.PagedSearch<ItemModel, ItemModel>();

        if (!string.IsNullOrEmpty(searchParams.SearchTerm))
        {
            query.Match(MongoDB.Entities.Search.Full, searchParams.SearchTerm).SortByTextScore();
        }

        if (!string.IsNullOrEmpty(searchParams.Seller))
        {
            query.Match(i => i.Seller == searchParams.Seller);
        }

        if (!string.IsNullOrEmpty(searchParams.Winner))
        {
            query.Match(i => i.Winner == searchParams.Winner);
        }

        query = searchParams.OrderBy switch
        {
            "make" => query.Sort(_ => _.Ascending(i => i.Make)),
            "new" => query.Sort(_ => _.Descending(i => i.CreatedAt)),
            _ => query.Sort(_ => _.Ascending(i => i.AuctionEnd))
        };

        query = searchParams.FilterBy switch
        {
            "finished" => query.Match(i => i.AuctionEnd < DateTime.UtcNow),
            "endingSoon" => query.Match(i =>
            i.AuctionEnd < DateTime.UtcNow.AddHours(6) && i.AuctionEnd > DateTime.UtcNow),
            _ => query.Match(i => i.AuctionEnd > DateTime.UtcNow)
        };

        query.PageNumber(searchParams.PageNumber);
        query.PageSize(searchParams.PageSize);

        var result = await query.ExecuteAsync(cancellationToken);

        return Ok(new
        {
            result = result.Results,
            pageCount = result.PageCount,
            totalCount = result.TotalCount
        });
    }

}

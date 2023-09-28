namespace Auctions.Api.Helpers;
public sealed class MapperProfiles : Profile
{
    public MapperProfiles()
    {
        CreateMap<AuctionEntity, AuctionResponse>().IncludeMembers(a => a.Item);
        CreateMap<ItemEntity, AuctionResponse>();
        CreateMap<CreateAuctionRequest, AuctionEntity>()
            .ForMember(d => d.Item, o => o.MapFrom(s => s));
        CreateMap<CreateAuctionRequest, ItemEntity>();
    }
}

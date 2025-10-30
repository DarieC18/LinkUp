using AutoMapper;
using LinkUp.Application.DTOs.Battleship;
using LinkUp.Domain.Entities.Battleship;

namespace LinkUp.Application.Mapping
{
    public class BattleshipProfile : Profile
    {
        public BattleshipProfile()
        {
            CreateMap<BattleshipGame, ActiveGameListItemDto>()
                .ForMember(d => d.GameId, m => m.MapFrom(s => s.Id));

            CreateMap<BattleshipGame, GameHistoryItemDto>()
                .ForMember(d => d.GameId, m => m.MapFrom(s => s.Id));

            CreateMap<GameSummaryDto, GameSummaryDto>();
        }
    }
}

using AutoMapper;
using FootScout_PostgreSQL.WebAPI.Entities;
using FootScout_PostgreSQL.WebAPI.Models.DTOs;

namespace FootScout_PostgreSQL.WebAPI.DbManager
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<RegisterDTO, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.SecurityStamp, opt => opt.MapFrom(src => Guid.NewGuid().ToString()));
            CreateMap<User, UserDTO>();
            CreateMap<UserDTO, User>();
            CreateMap<User, UserUpdateDTO>();
            CreateMap<UserUpdateDTO, User>();
            CreateMap<User, UserResetPasswordDTO>();
            CreateMap<UserResetPasswordDTO, User>();
            CreateMap<ClubHistoryCreateDTO, ClubHistory>()
                .ForMember(dest => dest.Achievements, opt => opt.MapFrom(src => src.Achievements));
            CreateMap<AchievementsDTO, Achievements>();
            CreateMap<Achievements, AchievementsDTO>();
            CreateMap<ClubHistory, ClubHistoryCreateDTO>();
            CreateMap<SalaryRange, SalaryRangeDTO>();
            CreateMap<SalaryRangeDTO, SalaryRange>();
            CreateMap<PlayerAdvertisement, PlayerAdvertisementCreateDTO>();
            CreateMap<PlayerAdvertisementCreateDTO, PlayerAdvertisement>();
            CreateMap<FavoritePlayerAdvertisement, FavoritePlayerAdvertisementCreateDTO>();
            CreateMap<FavoritePlayerAdvertisementCreateDTO, FavoritePlayerAdvertisement>();
            CreateMap<ClubOffer, ClubOfferCreateDTO>();
            CreateMap<ClubOfferCreateDTO, ClubOffer>();
            CreateMap<ClubAdvertisement, ClubAdvertisementCreateDTO>();
            CreateMap<ClubAdvertisementCreateDTO, ClubAdvertisement>();
            CreateMap<FavoriteClubAdvertisement, FavoriteClubAdvertisementCreateDTO>();
            CreateMap<FavoriteClubAdvertisementCreateDTO, FavoriteClubAdvertisement>();
            CreateMap<PlayerOffer, PlayerOfferCreateDTO>();
            CreateMap<PlayerOfferCreateDTO, PlayerOffer>();
            CreateMap<Problem, ProblemCreateDTO>();
            CreateMap<ProblemCreateDTO, Problem>();
            // Chat
            CreateMap<Chat, ChatCreateDTO>();
            CreateMap<ChatCreateDTO, Chat>();
            CreateMap<Message, MessageSendDTO>();
            CreateMap<MessageSendDTO, Message>();
        }
    }
}
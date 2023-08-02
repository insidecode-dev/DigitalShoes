
using AutoMapper;
using DigitalShoes.Domain.DTOs;
using DigitalShoes.Domain.Entities;

namespace DigitalShoes.Domain
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<CommentForInsertionDTO, Comment>().ForMember(dest => dest.ID, opt => opt.Ignore());
            //CreateMap<VillaNumberDTO, VillaNumberUpdateDTO>().ReverseMap();
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
        }
    }
}


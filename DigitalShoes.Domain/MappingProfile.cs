
using AutoMapper;
using DigitalShoes.Domain.DTOs.AuthDTOs;
using DigitalShoes.Domain.DTOs.HashtagDtos;
using DigitalShoes.Domain.DTOs.ImageDTOs;
using DigitalShoes.Domain.DTOs.ProductDTOs;
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

            // shoe
            CreateMap<Shoe, ShoeCreateDTO>().ReverseMap();

            // hashtag
            CreateMap<Hashtag, HashtagDTO>().ReverseMap();

            // image
            CreateMap<Image, ImageCreateDTO>().ReverseMap();
        }
    }
}


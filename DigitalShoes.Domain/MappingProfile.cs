using AutoMapper;
using DigitalShoes.Domain.DTOs.AuthDTOs;
using DigitalShoes.Domain.DTOs.CategoryDTOs;
using DigitalShoes.Domain.DTOs.HashtagDtos;
using DigitalShoes.Domain.DTOs.ImageDTOs;
using DigitalShoes.Domain.DTOs.ShoeDTOs;
using DigitalShoes.Domain.Entities;

namespace DigitalShoes.Domain
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            //CreateMap<CommentForInsertionDTO, Comment>().ForMember(dest => dest.ID, opt => opt.Ignore());
            //CreateMap<VillaNumberDTO, VillaNumberUpdateDTO>().ReverseMap();
            
            // auth
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();

            // shoe
            CreateMap<Shoe, ShoeCreateDTO>().ReverseMap();
            CreateMap<Shoe, ShoeDTO>().ReverseMap();
            CreateMap<Shoe, ShoeUpdateDTO>().ReverseMap();
            
            CreateMap<Shoe, ShoeGetDTO>()
            .ForMember(dest => dest.Hashtag, opt => opt.Ignore())
            .AfterMap((src, dest) =>
            {                
                dest.Hashtag = src.ShoeHashtags
                    .Select(sh => sh.Hashtag.Text)
                    .ToList();
            });

            // hashtag
            CreateMap<Hashtag, HashtagDTO>().ReverseMap();

            // image
            CreateMap<Image, ImageCreateDTO>().ReverseMap();
            CreateMap<Image, ImageDTO>().ReverseMap();

            // category
            CreateMap<Category, CategoryCreateDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();
        }
    }
}


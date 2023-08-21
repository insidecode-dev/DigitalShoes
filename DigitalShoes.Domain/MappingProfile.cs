using AutoMapper;
using DigitalShoes.Domain.DTOs.AuthDTOs;
using DigitalShoes.Domain.DTOs.CartItemDTOs;
using DigitalShoes.Domain.DTOs.CategoryDTOs;
using DigitalShoes.Domain.DTOs.HashtagDtos;
using DigitalShoes.Domain.DTOs.ImageDTOs;
using DigitalShoes.Domain.DTOs.PaymentDTOs;
using DigitalShoes.Domain.DTOs.ReviewDTOs;
using DigitalShoes.Domain.DTOs.ShoeDTOs;
using DigitalShoes.Domain.Entities;

namespace DigitalShoes.Domain
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            // auth
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();

            // shoe
            CreateMap<Shoe, ShoeCreateDTO>().ReverseMap();            
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
            CreateMap<Hashtag, HashtagGetDTO>()
                .ForMember(dest => dest.ShoeGetDTO, opt => opt.Ignore());
            //.AfterMap((src, dest) =>
            //{
            //    dest.ShoeGetDTO = src.ShoeHashtags.Select(x=>x.Shoe)
            //});

            // image
            CreateMap<Image, ImageCreateDTO>().ReverseMap();
            CreateMap<Image, ImageDTO>().ReverseMap();

            // category
            CreateMap<Category, CategoryCreateDTO>().ReverseMap();
            CreateMap<Category, CategoryDTO>().ReverseMap();

            // cart
            CreateMap<CartItem, CartItemCreateDTO>().ReverseMap();
            CreateMap<CartItem, CartItemGetDTO>().ReverseMap();
            CreateMap<CartItem, CartItemUpdateDTO>().ReverseMap();

            // review 
            CreateMap<Review, ReviewCreateDTO>().ReverseMap();
            CreateMap<Review, ReviewGetDTO>().ReverseMap();
            CreateMap<Review, ReviewUpdateDTO>().ReverseMap();

            // payment
            CreateMap<Payment, PaymentGetDTO>()                
            .ForMember(dest => dest.PaymentObjectsGetDTO, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentId, opt => opt.MapFrom(src => src.Id))
            .AfterMap((src, dest) =>
            {
                dest.PaymentObjectsGetDTO = src.PaymentObjects
                    .Select(po => new PaymentObjectGetDTO
                    {
                        ShoeId = po.ShoeId,
                        ItemsCount = po.ItemsCount,
                        Price = po.Price
                    })
                    .ToList();
            })
            .ReverseMap();
            CreateMap<PaymentObject, PaymentObjectGetDTO>().ReverseMap();

        }
    }
}


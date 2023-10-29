using AutoMapper;
using KittyShop.Data.Entities;
using KittyShop.Models;


namespace KittyShop.Data
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<RegisterModel, User>().ReverseMap();
            CreateMap<LoginModel, User>().ReverseMap();
            CreateMap<EditProfileModel, User>().ReverseMap();
            CreateMap<Product, CatModel>().ReverseMap();
        }
    }
}

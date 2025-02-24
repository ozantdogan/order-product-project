using AutoMapper;
using OTD.Core.Entities;
using OTD.Core.Models.Responses;

namespace OTD.Core
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        { 
            CreateMap<Product, ProductResponse>();
            CreateMap<Order, OrderResponse>();
        }
    }
}

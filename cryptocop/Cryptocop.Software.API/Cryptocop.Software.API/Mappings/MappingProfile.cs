using AutoMapper;
using Cryptocop.Software.API.Models.DTOs;
using Cryptocop.Software.API.Models.Entities;

namespace Cryptocop.Software.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Address, AddressDto>()
                .ForSourceMember(x => x.UserId, opt => opt.DoNotValidate());
            CreateMap<Order, OrderDto>()
                .ForMember(o => o.CreditCard, opt => opt.MapFrom(src => src.MaskedCreditCard))
                .ForMember(o => o.OrderDate, opt => opt.MapFrom(src => src.OrderDate.ToString("MM.dd.yyyy")))
                .ForSourceMember(p => p.UserId, opt => opt.DoNotValidate());
            CreateMap<OrderItem, OrderItemDto>()
                .ForSourceMember(o => o.OrderId, y => y.DoNotValidate())
                .ForSourceMember(o => o.Order, y => y.DoNotValidate());
            CreateMap<PaymentCard, PaymentCardDto>()
                .ForSourceMember(p => p.UserId, opt => opt.DoNotValidate());
            CreateMap<ShoppingCartItem, ShoppingCartItemDto>()
                .ForMember(c => c.TotalPrice, src => src.MapFrom(src => src.UnitPrice * src.Quantity))
                .ForSourceMember(c => c.ShoppingCartId, opt => opt.DoNotValidate());
            CreateMap<User, UserDto>()
                .ForSourceMember(u => u.Addresses, opt => opt.DoNotValidate())
                .ForSourceMember(u => u.PaymentCards, opt => opt.DoNotValidate())
                .ForSourceMember(u => u.Orders, opt => opt.DoNotValidate())
                .ForSourceMember(u => u.ShoppingCart, opt => opt.DoNotValidate());
        }
    }
}

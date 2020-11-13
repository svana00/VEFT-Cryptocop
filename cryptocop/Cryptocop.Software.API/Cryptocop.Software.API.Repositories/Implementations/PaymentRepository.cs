using System.Collections.Generic;
using Cryptocop.Software.API.Repositories.Interfaces;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Models.DTOs;
using Cryptocop.Software.API.Repositories.Contexts;
using Cryptocop.Software.API.Models.Entities;
using System.Linq;
using AutoMapper;

namespace Cryptocop.Software.API.Repositories.Implementations
{
    public class PaymentRepository : IPaymentRepository
    {
        private IMapper _mapper;
        private readonly CryptocopDbContext _dbContext;

        public PaymentRepository(CryptocopDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public void AddPaymentCard(string email, PaymentCardInputModel paymentCard)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            var newPaymentCard = new PaymentCard
            {
                CardholderName = paymentCard.CardholderName,
                CardNumber = paymentCard.CardNumber,
                Month = paymentCard.Month,
                Year = paymentCard.Year,
                UserId = user.Id,
            };
            _dbContext.PaymentCards.Add(newPaymentCard);
            _dbContext.SaveChanges();
        }

        public IEnumerable<PaymentCardDto> GetStoredPaymentCards(string email)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Email == email);

            // Map payment cards to dto
            var paymentCards = _mapper.Map<IEnumerable<PaymentCardDto>>(_dbContext.PaymentCards.Where(p => p.UserId == user.Id));
            return paymentCards;
        }
    }
}
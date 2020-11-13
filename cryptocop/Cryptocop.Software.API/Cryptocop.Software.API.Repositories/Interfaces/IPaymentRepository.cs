using System.Collections.Generic;
using Cryptocop.Software.API.Models.InputModels;
using Cryptocop.Software.API.Models.DTOs;

namespace Cryptocop.Software.API.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        void AddPaymentCard(string email, PaymentCardInputModel paymentCard);
        IEnumerable<PaymentCardDto> GetStoredPaymentCards(string email);
    }
}
namespace Cryptocop.Software.API.Repositories.Helpers
{
    public class PaymentCardHelper
    {
        public static string MaskPaymentCard(string paymentCardNumber)
        {
            return paymentCardNumber.Substring(paymentCardNumber.Length - 4).PadLeft(paymentCardNumber.Length, '*');
        }
    }
}
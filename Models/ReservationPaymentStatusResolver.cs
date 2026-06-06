namespace ProjIS.Models;

public class ReservationPaymentStatusResolver
{
    public PaymentStatus GetStatus(PaymentMethod paymentMethod)
    {
        if (paymentMethod == PaymentMethod.Card)
        {
            return PaymentStatus.Paid;
        }

        return PaymentStatus.Pending;
    }
}
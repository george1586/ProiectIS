namespace ProjIS.Models;

public class CashPaymentValidator
{
    public bool CanValidate(Reservation reservation)
    {
        if (reservation.PaymentMethod != PaymentMethod.Cash)
        {
            return false;
        }

        if (reservation.PaymentStatus != PaymentStatus.Pending)
        {
            return false;
        }

        return true;
    }

    public void Validate(Reservation reservation, int staffId)
    {
        if (!CanValidate(reservation))
        {
            return;
        }

        reservation.PaymentStatus = PaymentStatus.Paid;
        reservation.ValidatedByStaff = staffId;
        reservation.ValidatedAt = DateTime.UtcNow;
    }
}
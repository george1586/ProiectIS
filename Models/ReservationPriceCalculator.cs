namespace ProjIS.Models;

public class ReservationPriceCalculator
{
    private const decimal MealPrice = 30m;
    private const decimal ExtraLuggagePrice = 80m;
    private const decimal RoundTripDiscount = 0.05m;
    private const decimal LastMinuteDiscount = 0.40m;

    public decimal Calculate(
        Flight outboundFlight,
        FlightClass outboundClass,
        Flight? returnFlight,
        FlightClass? returnClass,
        int adultsCount,
        int childrenCount,
        int seniorsCount,
        bool hasMeal,
        bool hasExtraLuggage,
        DateOnly outboundDate,
        DateOnly? returnDate)
    {
        int passengersCount = adultsCount + childrenCount + seniorsCount;

        if (passengersCount <= 0)
        {
            return 0;
        }

        decimal total = GetPriceByClass(outboundFlight, outboundClass) * passengersCount;

        if (returnFlight != null && returnClass != null && returnDate != null)
        {
            total += GetPriceByClass(returnFlight, returnClass.Value) * passengersCount;
            total -= total * RoundTripDiscount;
        }

        if (hasMeal)
        {
            total += MealPrice * passengersCount;
        }

        if (hasExtraLuggage)
        {
            total += ExtraLuggagePrice * passengersCount;
        }

        if (IsLastMinute(outboundDate, outboundFlight.DepartureTime))
        {
            total -= total * LastMinuteDiscount;
        }

        return Math.Round(total, 2);
    }

    private decimal GetPriceByClass(Flight flight, FlightClass flightClass)
    {
        return flightClass switch
        {
            FlightClass.Economy => flight.EconomyPrice,
            FlightClass.Business => flight.BusinessPrice,
            FlightClass.First => flight.FirstClassPrice,
            _ => flight.EconomyPrice
        };
    }

    private bool IsLastMinute(DateOnly date, TimeOnly time)
    {
        DateTime flightDateTime = date.ToDateTime(time);
        DateTime now = DateTime.Now;

        return flightDateTime > now && flightDateTime <= now.AddHours(48);
    }
}
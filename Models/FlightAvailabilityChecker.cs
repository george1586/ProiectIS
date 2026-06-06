namespace ProjIS.Models;

public class FlightAvailabilityChecker
{
    public bool IsAvailableOnDate(Flight flight, DateOnly date)
    {
        int dayOfWeek = ((int)date.DayOfWeek + 6) % 7 + 1;

        bool correctDay = flight.DaysOfWeek.Contains(dayOfWeek);

        if (!correctDay)
        {
            return false;
        }

        if (flight.FlightType == FlightType.Seasonal)
        {
            if (flight.SeasonStart == null || flight.SeasonEnd == null)
            {
                return false;
            }

            return date >= flight.SeasonStart && date <= flight.SeasonEnd;
        }

        return true;
    }
}
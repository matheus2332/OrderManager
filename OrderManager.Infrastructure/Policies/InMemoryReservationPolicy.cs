namespace OrderManager.Infrastructure.Policies;

public class InMemoryReservationPolicy : IReservationPolicy
{
    public TimeSpan GetReservationTime()
    {
        return TimeSpan.FromMinutes(1);
    }
}
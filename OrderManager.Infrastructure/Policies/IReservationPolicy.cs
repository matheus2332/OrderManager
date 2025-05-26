namespace OrderManager.Infrastructure.Policies;

public interface IReservationPolicy
{
    TimeSpan GetReservationTime();
}
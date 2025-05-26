namespace OrderManager.Application.Interfaces;

public interface IOrderExpirationPublisher : IDisposable
{
    void PublishExpiration(int orderId);
}
using OrderManager.Application.Dtos;

namespace OrderManager.Application.Interfaces;

public interface IOrderService
{
    CreateOrderResultDto CreateOrder(int quantity);
    bool CompleteOrder(int id);
    void ExpireOrder(int id);
    List<OrderDto> GetAll();
}
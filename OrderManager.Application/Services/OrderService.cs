using OrderManager.Application.Dtos;
using OrderManager.Application.Interfaces;
using OrderManager.Domain.Entities;

namespace OrderManager.Application.Services;

public class OrderService(IInventoryService inventory, IOrderExpirationPublisher publisher) : IOrderService
{
    private readonly Dictionary<int, Order> _orders = new();
    private int _nextId = 1;

    public CreateOrderResultDto CreateOrder(int quantity)
    {
        var validationError = ValidateOrder(quantity);
        if (validationError != null) return CreateError(validationError);

        var order = new Order { Id = _nextId++, Quantity = quantity };
        _orders[order.Id] = order;
        
        publisher.PublishExpiration(order.Id);
        
        return new CreateOrderResultDto
        {
            Success = true,
            Order = ToDto(order)
        };
    }

    public bool CompleteOrder(int id)
    {
        if (_orders.TryGetValue(id, out var o) && !o.Completed)
        {
            o.Completed = true;
            inventory.ConfirmReservation(o.Quantity);
            return true;
        }
        return false;
    }

    public void ExpireOrder(int id)
    {
        if (_orders.TryGetValue(id, out var o) && !o.Completed)
        {
            inventory.Return(o.Quantity);
            _orders.Remove(id);
        }
    }

    public List<OrderDto> GetAll() => _orders.Values.Select(ToDto).OrderByDescending(x => x.CreatedAt).ToList();

    private OrderDto ToDto(Order order) => new()
    {
        Id = order.Id,
        Quantity = order.Quantity,
        Completed = order.Completed,
        CreatedAt = order.CreatedAt
    };

    private string? ValidateOrder(int quantity)
    {
        if (quantity <= 0) return "Invalid quantity";
        
        if (!inventory.Reserve(quantity)) return "Insufficient stock";

        return null;
    }

    private CreateOrderResultDto CreateError(string message) =>
        new CreateOrderResultDto
        {
            Success = false,
            ErrorMessage = message
        };
}

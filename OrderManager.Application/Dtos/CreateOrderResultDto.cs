using OrderManager.Domain.Entities;

namespace OrderManager.Application.Dtos;

public class CreateOrderResultDto
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public OrderDto? Order { get; set; }
}
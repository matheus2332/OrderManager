namespace OrderManager.Application.Dtos;

public class OrderDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public bool Completed { get; set; }
    public DateTime CreatedAt { get; set; }
}

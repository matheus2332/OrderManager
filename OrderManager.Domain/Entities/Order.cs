namespace OrderManager.Domain.Entities;

public class Order
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public bool Completed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
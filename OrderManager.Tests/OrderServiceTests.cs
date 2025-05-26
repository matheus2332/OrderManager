using Moq;
using OrderManager.Application.Interfaces;
using OrderManager.Application.Services;

namespace OrderManager.Tests;

public class OrderServiceTests
{
    private readonly Mock<IInventoryService> _inventoryMock;
    private readonly Mock<IOrderExpirationPublisher> _publisherMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _inventoryMock = new Mock<IInventoryService>();
        _publisherMock = new Mock<IOrderExpirationPublisher>();
        _orderService = new OrderService(_inventoryMock.Object, _publisherMock.Object);
    }

    [Fact]
    public void Initial_Stock_Should_Be_100()
    {
        var inventory = new InMemoryInventoryService();
        Assert.Equal(100, inventory.GetAvailable());
    }

    [Fact]
    public void Reserve_Should_Decrease_Stock_When_Available()
    {
        var quantity = 10;
        var inventory = new InMemoryInventoryService();
        var result = inventory.Reserve(quantity);

        Assert.True(result);
        Assert.Equal(90, inventory.GetAvailable());
        Assert.Equal(quantity, inventory.GetReserved());
    }

    [Fact]
    public void Reserve_Should_Fail_When_Stock_Insufficient()
    {
        var inventory = new InMemoryInventoryService();
        var result = inventory.Reserve(101);

        Assert.False(result);
        Assert.Equal(100, inventory.GetAvailable());
    }

    [Fact]
    public void Return_Should_Increase_Stock()
    {
        var inventory = new InMemoryInventoryService();
        inventory.Reserve(10);
        inventory.Return(7);

        Assert.Equal(97, inventory.GetAvailable());
        Assert.Equal(3, inventory.GetReserved());
    }

    [Fact]
    public void Should_Create_Order_When_Stock_Available()
    {
        _inventoryMock.Setup(inv => inv.Reserve(It.IsAny<int>())).Returns(true);

        var order = _orderService.CreateOrder(5);

        Assert.NotNull(order);
        _inventoryMock.Verify(inv => inv.Reserve(5), Times.Once);
    }

    [Fact]
    public void Should_Not_Create_Order_When_Stock_Insufficient()
    {
        _inventoryMock.Setup(inv => inv.Reserve(It.IsAny<int>())).Returns(false);

        var order = _orderService.CreateOrder(101);

        Assert.False(order.Success);
        Assert.NotEmpty(order.ErrorMessage);
        _inventoryMock.Verify(inv => inv.Reserve(101), Times.Once);
    }

    [Fact]
    public void Should_Not_Create_Order_When_Invalid_Quantity()
    {
        _inventoryMock.Setup(inv => inv.Reserve(It.IsAny<int>())).Returns(false);

        var order = _orderService.CreateOrder(0);

        Assert.False(order.Success);
        Assert.NotEmpty(order.ErrorMessage);
    }

    [Fact]
    public void Should_Not_Create_Order_When_Negative_Quantity()
    {
        _inventoryMock.Setup(inv => inv.Reserve(It.IsAny<int>())).Returns(false);

        var order = _orderService.CreateOrder(-5);

        Assert.False(order.Success);
        Assert.NotEmpty(order.ErrorMessage);
    }

    [Fact]
    public void Should_Expire_Order_And_Return_Stock()
    {
        _inventoryMock.Setup(inv => inv.Reserve(It.IsAny<int>())).Returns(true);

        var result = _orderService.CreateOrder(10);

        if (result.Order?.Id is null)
        {
            Assert.Fail("order not created");
        }

        _orderService.ExpireOrder(result.Order.Id);

        _inventoryMock.Verify(inv => inv.Return(10), Times.Once);

        var inventory = new InMemoryInventoryService();
        Assert.Equal(100, inventory.GetAvailable());
        Assert.Equal(0, inventory.GetReserved());
    }

    [Fact]
    public void Should_Call_Publisher_When_Order_Is_Created()
    {
        var inventoryMock = new Mock<IInventoryService>();
        inventoryMock.Setup(i => i.Reserve(It.IsAny<int>())).Returns(true);

        var publisherMock = new Mock<IOrderExpirationPublisher>();

        var service = new OrderService(inventoryMock.Object, publisherMock.Object);

        var result = service.CreateOrder(5);

        Assert.True(result.Success);
        publisherMock.Verify(p => p.PublishExpiration(It.IsAny<int>()), Times.Once);
    }
}
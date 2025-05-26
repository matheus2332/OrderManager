using OrderManager.Application.Interfaces;

namespace OrderManager.Application.Services;

public class InMemoryInventoryService : IInventoryService
{
    private int _stock = 100;
    private int _reserved;

    public int GetAvailable() => _stock;
    public int GetReserved() => _reserved;

    public bool Reserve(int quantity)
    {
        if (_stock < quantity) return false;
        _stock -= quantity;
        _reserved += quantity;
        return true;
    }

    public void Return(int quantity)
    {
        _stock += quantity;
        _reserved -= quantity;
    }

    public void ConfirmReservation(int quantity)
    {
        _reserved -= quantity;
    }
}
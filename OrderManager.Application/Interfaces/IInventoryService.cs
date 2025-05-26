using System.Collections.Generic;

namespace OrderManager.Application.Interfaces;

public interface IInventoryService
{
    int GetAvailable();
    int GetReserved();
    bool Reserve(int quantity);
    void Return(int quantity);
    void ConfirmReservation(int quantity);
}
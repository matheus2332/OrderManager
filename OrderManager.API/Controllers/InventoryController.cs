using Microsoft.AspNetCore.Mvc;
using OrderManager.Application.Interfaces;

namespace OrderManager.API.Controllers;

[ApiController]
[Route("[controller]")]
public class InventoryController(IInventoryService inventoryService) : ControllerBase
{
    [HttpGet("available")]
    public IActionResult GetAvailableStock()
    {
        return Ok(new { available = inventoryService.GetAvailable() });
    }

    [HttpGet("reserved")]
    public IActionResult GetReservedStock()
    {
        return Ok(new { reserved = inventoryService.GetReserved() });
    }

    [HttpGet]
    public IActionResult GetSummary()
    {
        return Ok(new
        {
            Available = inventoryService.GetAvailable(),
            Reserved = inventoryService.GetReserved()
        });
    }
}
using backend_ecommerce.repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using backend_ecommerce.Models;
using backend_ecommerce.repository;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase {
    private readonly OrderRepository _repository;

    public OrderController(OrderRepository repository) {
        _repository = repository;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Policy = "User")]

    public async Task<IActionResult> AddOrderAsync(AddOrderDTO orderDto) {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        await _repository.AddOrderAsync(orderDto);

        return CreatedAtAction(nameof(AddOrderDTO), new { orderId = orderDto.Id}, orderDto);
    }
}
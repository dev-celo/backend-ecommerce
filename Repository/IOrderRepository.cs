using backend_ecommerce.Models;

namespace backend_ecommerce.repository
{
    public interface IOrderRepository
    {
        Task<GetOrderDTO?> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<GetOrderDTO>> GetOrdersByUserIdAsync(int userId);
        Task AddOrderAsync(AddOrderDTO orderDto);
        Task UpdateOrderStatusAsync(int orderId, string status);
    }
}
using backend_ecommerce.Models;

namespace backend_ecommerce.repository
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
        Task AddOrderAsync(Order order);
        Task UpdateOrderStatusAsync(int orderId, string status);
    }
}
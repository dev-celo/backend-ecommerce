using backend_ecommerce.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;


namespace backend_ecommerce.repository
{
    public class OrderRepository : IOrderRepository
    {

        private readonly EcommerceContext _context;
        public OrderRepository(EcommerceContext context)
        {
            _context = context;
        }

        public async Task<GetOrderDTO?> GetOrderByIdAsync(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");
            }

            return new GetOrderDTO
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = DateTime.UtcNow,
                Status = order.status,
                Total = order.total,
                OrderItems = order.OrderItems.Select(order => new GetOrderItemDTO
                {
                    ProductId = order.ProductId,
                    ProductName = order.Product.Name,
                    Quantity = order.Quantity,
                    UnitPrice = order.UnitPrice
                }).ToList()
            };
        }

        public async Task<IEnumerable<GetOrderDTO>> GetOrdersByUserIdAsync(int userId)
        {
            var order = await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(oi => oi.OrderItems)
                .ThenInclude(oi => oi.Product)
                .ToListAsync();

            return order.Select((order) => new GetOrderDTO
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = DateTime.UtcNow,
                Status = order.status,
                Total = order.total,
                OrderItems = order.OrderItems.Select((orders) => new GetOrderItemDTO
                {
                    ProductId = orders.ProductId,
                    ProductName = orders.Product.Name,
                    Quantity = orders.Quantity,
                    UnitPrice = orders.UnitPrice
                }).ToList(),
            }).ToList();
        }

        public async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.status = status;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddOrderAsync(AddOrderDTO orderDto)
        {
            try
            {
                // Criação de um novo objeto Order a partir do DTO
                var order = new Order
                {
                    UserId = orderDto.UserId,
                    OrderDate = DateTime.UtcNow,
                    status = orderDto.status,
                    OrderItems = orderDto.OrderItems.Select((oi) => new OrderItem
                    {
                        ProductId = oi.ProductId,
                        Quantity = oi.Quantity,
                        UnitPrice = oi.UnitPrice,
                    }).ToList(),
                };

                // Adiciona o pedido ao banco de dados
                await _context.Orders.AddAsync(order);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Tratar exceções, logar ou relatar erro
                throw new Exception("Erro ao adicionar pedido", ex);
            }
        }
    }
}
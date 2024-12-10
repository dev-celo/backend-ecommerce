using System.Text.Json.Serialization;

namespace backend_ecommerce.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public DateTime OrderDate { get; set; }
        public string status { get; set; }
        public decimal total { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }

    }
}
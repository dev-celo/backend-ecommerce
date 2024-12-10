using System.Text.Json.Serialization;

namespace backend_ecommerce.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? PasswordHash { get; set; }
        // Ignore a referência circular em Orders
        [JsonIgnore]
        public ICollection<Order>? Orders { get; set; }
        public string? Access { get; set; }
        public string? Salt { get; set; }

    }
}
using backend_ecommerce.Models;

public class OrderDto {
    public int Id;
    public DateTime OrderDate;
    public string Status;
    public decimal Total;
    public IEnumerable<OrderItemDto> OrderITem;
}

public class OrderItemDto {
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class AddOrderDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string status { get; set; }
    public IEnumerable<AddOrderItemDTO> OrderItems { get; set; }
}

public class AddOrderItemDTO
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class GetOrderItemDTO
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class GetOrderDTO
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; }
    public decimal Total { get; set; }
    public List<GetOrderItemDTO> OrderItems { get; set; }
}

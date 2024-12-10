// importar o EntityFramework para utilizar o DbContext
using System.Security.Cryptography;
using backend_ecommerce.Models;
using Microsoft.EntityFrameworkCore;
//cria uma classe pública responsável pelo contexto
public class EcommerceContext : DbContext
{
    /* cria um construtor que envia as configurações de inicialização
    para o construtor da classe pai DbContext */
    public EcommerceContext(DbContextOptions<EcommerceContext> options) : base(options)
    {

    }

    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Product> Products { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)    // Cada OrderItem tem um Product
            .WithMany()                  // Product não possui uma coleção de OrderItems
            .HasForeignKey(oi => oi.ProductId);  // A chave estrangeira é ProductId

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // buscamos o valor da connection string armazenada  nas variáveis de ambiente
        var connectionString = Environment.GetEnvironmentVariable("DOTNET_CONNECTION_STRING");
        // verificamos se o banco de dados já foi configurado
        if (!optionsBuilder.IsConfigured)
        {
            // usamos o método UseSqlServer e passamos a connectionString para ele
            optionsBuilder.UseSqlServer(connectionString);
        }
        base.OnConfiguring(optionsBuilder);
    }
}
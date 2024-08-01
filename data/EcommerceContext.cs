// importar o EntityFramework para utilizar o DbContext
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


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // verificamos se o banco de dados já foi configurado
        if (optionsBuilder.IsConfigured)
        {
            // buscamos o valor da connection string armazenada  nas variáveis de ambiente
            var connectionString = Environment.GetEnvironmentVariable("DOTNET_CONNECTION_STRING");

            // usamos o método UseSqlServer e passamos a connectionString para ele
            optionsBuilder.UseSqlServer(connectionString);
        }

        base.OnConfiguring(optionsBuilder);
    }
}
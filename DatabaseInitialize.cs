using Microsoft.EntityFrameworkCore;

public static class DatabaseInitializer
{
    public static void Initialize(EcommerceContext context)
    {
        context.Database.Migrate();
    }
}

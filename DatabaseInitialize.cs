public static class DatabaseInitializer
{
    public static void Initialize(EcommerceContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}

using System.Security.Claims;
using System.Text;
using Auth.Constants;
using backend_ecommerce.repository;
using backend_ecommerce.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<EcommerceContext>();
builder.Services.AddScoped<EcommerceContext>();
builder.Services.AddScoped<UserRepository>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configura o Serializador JSON para usar o ReferenceHandler.Preserve
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true; // opcional, para uma saída mais legível
    });

builder.Services.AddScoped<OrderRepository>();  // Remover duplicação

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurações de conjunto de políticas JWT Authorization
builder.Services.AddAuthorization(options => 
{
    options.AddPolicy("Admin", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
    options.AddPolicy("User", policy => policy.RequireClaim(ClaimTypes.Role, "User"));
    options.AddPolicy("Client", policy => policy.RequireClaim(ClaimTypes.Email));
    options.AddPolicy("SouthAmerica", policy => policy.RequireClaim(ClaimTypes.Country, SouthAmerica.Countries));
});

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.SaveToken = true;
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = false,  // Configurar em seguida
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true, 
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])) // Chave secreta
    };
});

builder.Services.AddScoped<TokenGenerator>();  // Verifique se está sendo usado

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EcommerceContext>();  // Alterado para camelCase
    DatabaseInitializer.Initialize(dbContext);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

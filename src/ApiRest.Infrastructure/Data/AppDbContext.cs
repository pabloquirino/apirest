using ApiRest.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiRest.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User>         Users         { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Product>      Products      { get; set; }
    public DbSet<Order>        Orders        { get; set; }
    public DbSet<OrderItem>    OrderItems    { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplica todas as IEntityTypeConfiguration do assembly automaticamente
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
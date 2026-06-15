using Microsoft.EntityFrameworkCore;
using NorthWindTracker.Domain;

namespace NorthWindTracker.Infrastructure.Data;

public class NorthwindDbContext(DbContextOptions<NorthwindDbContext> options) : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(e =>
        {
            e.ToTable("Customers");
            e.HasKey(c => c.CustomerId);
            e.Property(c => c.CustomerId).HasColumnName("CustomerID").HasMaxLength(5);
            e.Property(c => c.CompanyName).HasColumnName("CompanyName");
            e.Property(c => c.ContactName).HasColumnName("ContactName");
            e.Property(c => c.City).HasColumnName("City");
            e.Property(c => c.Country).HasColumnName("Country");
            e.HasMany(c => c.Orders)
             .WithOne()
             .HasForeignKey(o => o.CustomerId);
        });

        modelBuilder.Entity<Order>(e =>
        {
            e.ToTable("Orders");
            e.HasKey(o => o.OrderId);
            e.Property(o => o.OrderId).HasColumnName("OrderID");
            e.Property(o => o.CustomerId).HasColumnName("CustomerID").HasMaxLength(5);
            e.Property(o => o.OrderDate).HasColumnName("OrderDate");
            e.HasMany(o => o.OrderDetails)
             .WithOne()
             .HasForeignKey(od => od.OrderId);
        });

        modelBuilder.Entity<OrderDetail>(e =>
        {
            e.ToTable("Order Details");
            e.HasKey(od => new { od.OrderId, od.ProductId });
            e.Property(od => od.OrderId).HasColumnName("OrderID");
            e.Property(od => od.ProductId).HasColumnName("ProductID");
            e.Property(od => od.UnitPrice).HasColumnName("UnitPrice").HasColumnType("money");
            e.Property(od => od.Quantity).HasColumnName("Quantity");
            e.Property(od => od.Discount).HasColumnName("Discount");
            e.HasOne(od => od.Product)
             .WithMany()
             .HasForeignKey(od => od.ProductId);
        });

        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("Products");
            e.HasKey(p => p.ProductId);
            e.Property(p => p.ProductId).HasColumnName("ProductID");
            e.Property(p => p.ProductName).HasColumnName("ProductName");
        });
    }
}

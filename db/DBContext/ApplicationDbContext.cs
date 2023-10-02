using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Task> Tasks { get; set; }

    public DbSet<Category> Categories { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Product>().Property(p => p.Description).HasMaxLength(500).IsRequired(false);
        builder.Entity<Product>().Property(p => p.Name).HasMaxLength(120).IsRequired();
        builder.Entity<Product>().Property(p => p.Code).HasMaxLength(20).IsRequired();
        builder.Entity<Category>().ToTable("Categories");

        builder.Entity<Task>().Property(t => t.Title).HasMaxLength(60).IsRequired(false);
        builder.Entity<Task>().Property(t => t.Description).HasMaxLength(254).IsRequired();
        builder.Entity<Task>().ToTable("tasks");
    }
}
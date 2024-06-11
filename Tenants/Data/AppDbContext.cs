using Microsoft.EntityFrameworkCore;

namespace Tenants.Data;

public class AppDbContext : DbContext
{
    private readonly ITenantService _tenantService;
    private string TenantId { get; set; }

    public AppDbContext(DbContextOptions options, ITenantService tenantService) : base(options)
    {
        _tenantService = tenantService;
        TenantId = _tenantService.GetCurrentTenant()?.Id;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = _tenantService.GetConnectionString();
        if (connectionString != null)
        {
            var dbProvider = _tenantService.GetDataBaseProvidor();
            if (dbProvider == "SqlServer")
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>().HasQueryFilter(p => p.TenantId == TenantId);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new CancellationToken())
    {
        foreach (var entry in ChangeTracker.Entries<IHaveTenant>().Where(e => e.State == EntityState.Added))
        {
            entry.Entity.TenantId = TenantId;
        }

        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }


    public DbSet<Product> Products { get; set; }
}
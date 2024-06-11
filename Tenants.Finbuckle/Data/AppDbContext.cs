using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Tenants.Finbuckle.Data;

public class AppDbContext : DbContext, IMultiTenantDbContext
{
    private readonly IConfiguration _configuration;
    public ITenantInfo? TenantInfo { get; }
    public TenantMismatchMode TenantMismatchMode { get; }
    public TenantNotSetMode TenantNotSetMode { get; }


    public AppDbContext(IConfiguration configuration,IMultiTenantContextAccessor<TenantInfo> multiTenantContextAccessor,
        DbContextOptions options) : base(options)
    {
        _configuration = configuration;
        TenantInfo = multiTenantContextAccessor.MultiTenantContext?.TenantInfo;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fluent api to configure multi-tenant
        modelBuilder.Entity<Product>().IsMultiTenant();
        // modelBuilder.ConfigureMultiTenant();
        base.OnModelCreating(modelBuilder);
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        this.EnforceMultiTenant();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        this.EnforceMultiTenant();
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
}
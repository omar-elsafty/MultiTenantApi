
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace PerTenantDb.Data;

public class AppDbContext : DbContext
{
    private readonly IConfiguration _configuration;
    public AppTenantInfo TenantInfo { get; }
    
    public AppDbContext(IConfiguration configuration,IMultiTenantContextAccessor<AppTenantInfo> multiTenantContextAccessor,
        DbContextOptions options) : base(options)
    {
        _configuration = configuration;
        TenantInfo = multiTenantContextAccessor.MultiTenantContext?.TenantInfo;
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
       var connectionSring = TenantInfo?.ConnectionString ??_configuration.GetConnectionString("DefaultConnection") ;
        optionsBuilder.UseSqlServer(connectionSring);
    }
    
    
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
}
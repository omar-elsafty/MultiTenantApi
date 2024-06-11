// using Finbuckle.MultiTenant.Abstractions;
// using Finbuckle.MultiTenant.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore;
//
// namespace Tenants.Finbuckle.Data;
//
// public class ModelDbContext : MultiTenantDbContext
// {
//     public ModelDbContext(IMultiTenantContextAccessor multiTenantContextAccessor) : base(multiTenantContextAccessor)
//     {
//     }
//     
//     public DbSet<Product> Products { get; set; }
//     public DbSet<Category> Categories { get; set; }
// }
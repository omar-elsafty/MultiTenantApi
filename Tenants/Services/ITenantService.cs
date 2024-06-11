namespace Tenants.Services;

public interface ITenantService
{
    string? GetDataBaseProvidor();
    string? GetConnectionString();
    Tenant? GetCurrentTenant();
}
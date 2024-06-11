using Microsoft.Extensions.Options;

namespace Tenants.Services;

class TenantService : ITenantService
{
    private readonly TenantSettings _tenantSettings;
    private Tenant? _tenant;

    public TenantService(IHttpContextAccessor contextAccessor, IOptions<TenantSettings> tenantSettings)
    {
        _tenantSettings = tenantSettings.Value;
        var httpContext = contextAccessor.HttpContext;
        if (httpContext == null) return;

        if (httpContext.Request.Headers.TryGetValue("TenantId", out var tenantId))
        {
            SetTenant(tenantId!);
        }
        else
        {
            throw new Exception("TenantId not found in the request header.");
        }
    }


    public string? GetDataBaseProvidor() => _tenantSettings.Defaults.DbProvidor;

    public string? GetConnectionString() => _tenant?.ConnectionString ?? _tenantSettings.Defaults.ConnectionString;

    public Tenant? GetCurrentTenant() => _tenant;

    private void SetTenant(string tenantId)
    {
        _tenant = _tenantSettings.Tenants.FirstOrDefault(t => t.Id == tenantId) ??
                  throw new Exception("Tenant not found.");

        _tenant.ConnectionString ??= _tenantSettings.Defaults.ConnectionString;
    }
}
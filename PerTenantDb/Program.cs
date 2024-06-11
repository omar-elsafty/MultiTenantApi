using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Microsoft.EntityFrameworkCore;
using PerTenantDb.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => { options.UseSqlServer(); });


builder.Services.AddMultiTenant<AppTenantInfo>()
    .WithHeaderStrategy("tenant")
    .WithConfigurationStore();


using var scope = builder.Services.BuildServiceProvider().CreateScope();
var tenantStore = scope.ServiceProvider.GetRequiredService<IMultiTenantStore<AppTenantInfo>>();

foreach (var tenant in tenantStore.GetAllAsync().Result)
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.SetConnectionString(tenant.ConnectionString);

    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/product/list", async (AppDbContext dbContext) => await dbContext.Products.ToListAsync())
    .WithOpenApi();


app.MapGet("/product/{id}",
        async (int id, AppDbContext dbContext) => await 
            dbContext.Products.FirstOrDefaultAsync(p => p.Id == id))
    .WithOpenApi();


app.MapPost("/product", async (ProductDto productdto, AppDbContext dbContext) =>
    {
        var product = new Product
        {
            Name = productdto.Name
        };
        await dbContext.Products.AddAsync(product);
        await dbContext.SaveChangesAsync();
    })
    .WithOpenApi();

app.UseMultiTenant();
app.Run();
using Finbuckle.MultiTenant;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddMultiTenant<TenantInfo>()
    .WithHeaderStrategy("tenant")
    .WithConfigurationStore();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/product/list", async (AppDbContext dbContext) =>  await dbContext.Products.ToListAsync())
    .WithOpenApi();



app.MapGet("/product/{id}", async (int id ,AppDbContext dbContext) =>  await dbContext.Products.FirstOrDefaultAsync(p => p.Id == id))
    .WithOpenApi();


app.MapPost("/product", async (ProductDto  productdto, AppDbContext dbContext) =>
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
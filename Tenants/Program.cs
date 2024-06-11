using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.Configure<TenantSettings>(builder.Configuration.GetSection(nameof(TenantSettings)));
builder.Services.AddScoped<ITenantService, TenantService>();

TenantSettings? options = builder.Configuration.GetSection(nameof(TenantSettings)).Get<TenantSettings>();

var defaultProvider = options?.Defaults?.DbProvidor ?? "SqlServer";

if (defaultProvider == "SqlServer")
{
    builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer());
}

foreach (var tenant in options.Tenants)
{
    var connectionString = tenant.ConnectionString ?? options.Defaults.ConnectionString;
    using (var scope = builder.Services.BuildServiceProvider().CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.SetConnectionString(connectionString);

        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
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

app.Run();


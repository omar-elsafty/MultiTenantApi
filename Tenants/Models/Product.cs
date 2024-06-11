
public class Product : IHaveTenant
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string TenantId { get; set; }
}

public class ProductDto 
{
    public int Id { get; set; }
    public string Name { get; set; }
}
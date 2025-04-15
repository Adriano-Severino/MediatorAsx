
public interface IProductRepository
{
    public Task<Product> Add(CreateProductCommand createProductCommand);
}
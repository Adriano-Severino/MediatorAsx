
namespace MediatorAsx.Samples
{
    public class ProductRepository : IProductRepository
    {
        public async Task<Product> Add(CreateProductCommand product)
        {
            var newProduct = new Product(product.Name, product.Price);
            Console.WriteLine($"Product created: {product.Name} with price: {product.Price}");
            return newProduct;
        }

    }
}
using Elasticsearch.API.Dtos;
using Elasticsearch.API.Repository;

namespace Elasticsearch.API.Services;

public class ProductService
{
    private readonly ProductRepository _productRepository;

    public ProductService(ProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task SaveAsync(ProductCreateDto request)
    {

    }
}

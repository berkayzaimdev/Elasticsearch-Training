using Elasticsearch.API.Models;
using Nest;

namespace Elasticsearch.API.Repository;

public class ProductRepository
{
    private readonly ElasticClient _client;

    public ProductRepository(ElasticClient client)
    {
        _client = client;
    }

    public async Task<Product?> SaveAsync(Product newProduct)
    {
        newProduct.Created = DateTime.Now;
        var response = await _client.IndexAsync(newProduct, x => x.Index("products"));

        if (response.IsValid)
        {
            newProduct.Id = response.Id;

            return newProduct;
        }

        return null;
    }
}

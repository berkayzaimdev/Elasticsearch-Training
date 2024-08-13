using Nest;

namespace Elasticsearch.API.Repository;

public class ProductRepository
{
    private readonly ElasticClient _client;

    public ProductRepository(ElasticClient client)
    {
        _client = client;
    }
}

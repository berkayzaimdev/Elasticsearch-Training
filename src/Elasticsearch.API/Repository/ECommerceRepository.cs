using Elastic.Clients.Elasticsearch;
using Elasticsearch.API.Models.ECommerces;
using System.Collections.Immutable;

namespace Elasticsearch.API.Repository;

public class ECommerceRepository
{
    private readonly ElasticsearchClient _client;
    private const string indexName = "kibana_sample_data_ecommerce";

    public ECommerceRepository(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task<ImmutableList<ECommerce>> TermQuery(string customerFirstName)
    {
        var result = await _client.SearchAsync<ECommerce>(s => s.Query(
            q => q.Term(
                t => t.Field("customer_first_name.keyword"!).Value(customerFirstName)
                )
            )
        );

        return result.Documents.ToImmutableList();
    }
}

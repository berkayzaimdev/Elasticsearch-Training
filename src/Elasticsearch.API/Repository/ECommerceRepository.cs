using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
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
        // 1. YOL
        //var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName).Query(
        //    q => q.Term(
        //        t => t.Field("customer_first_name.keyword"!).Value(customerFirstName)
        //        )
        //    )
        //);

        // 2. YOL
        var termQuery = new TermQuery("customer_first_name.keyword") { Value = customerFirstName, CaseInsensitive = true};
        var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
        .Query(termQuery));

        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> TermsQuery(List<string> customerFirstNames)
    {
        List<FieldValue> terms = [.. customerFirstNames];

        var termsQuery = new TermsQuery()
        {
            Field = "customer_first_name.keyword",
            Term = new TermsQueryField(terms.AsReadOnly())
        };

        // 1.yol
        var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
        .Query(termsQuery));

        // 2.yol
        var result2 = await _client.SearchAsync<ECommerce>(s =>
        s.Index(indexName)
        .Query(q => q
        .Terms(t => t
        .Field(c => c.CustomerFirstName.Suffix("keyword")).Term(new TermsQueryField(terms.AsReadOnly())))));

        foreach (var hit in result2.Hits) hit.Source.Id = hit.Id;

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> PrefixQuery(string customerFullName)
    {
        var result = await _client.SearchAsync<ECommerce>(s =>
            s.Index(indexName)
                .Query(q => q
                    .Prefix(p => p
                        .Field(c => c.CustomerFullName.Suffix("keyword")).Value(customerFullName))));

        return result.Documents.ToImmutableList();
    }
}

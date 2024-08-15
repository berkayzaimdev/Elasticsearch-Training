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

    public async Task<ImmutableList<ECommerce>> TermQueryAsync(string customerFirstName)
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

        result = SetIds(result);

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> TermsQueryAsync(List<string> customerFirstNames)
    {
        List<FieldValue> terms = [.. customerFirstNames];

        // 1.yol
        //var termsQuery = new TermsQuery()
        //{
        //    Field = "customer_first_name.keyword",
        //    Term = new TermsQueryField(terms.AsReadOnly())
        //};


        //var result = await _client.SearchAsync<ECommerce>(s => s.Index(indexName)
        //.Query(termsQuery));

        // 2.yol
        var result = await _client.SearchAsync<ECommerce>(s =>
        s.Index(indexName)
            .Query(q => q
                .Terms(t => t
                    .Field(c => c.CustomerFirstName.Suffix("keyword"))
                        .Term(new TermsQueryField(terms.AsReadOnly())))));

        result = SetIds(result);

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> PrefixQueryAsync(string customerFullName)
    {
        var result = await _client.SearchAsync<ECommerce>(s =>
            s.Index(indexName)
                .Query(q => q
                    .Prefix(p => p
                        .Field(c => c.CustomerFullName.Suffix("keyword")).Value(customerFullName))));

        return result.Documents.ToImmutableList();
    }
    public async Task<ImmutableList<ECommerce>> RangeQueryAsync(double fromPrice, double toPrice)
    {
        var result = await _client.SearchAsync<ECommerce>(s =>
            s.Index(indexName)
                .Query(q => q
                    .Range(r => r
                        .NumberRange(n => n
                            .Field(x => x.TaxfulTotalPrice)
                                .Gte(fromPrice).Lte(toPrice)))));

        return result.Documents.ToImmutableList();
    }
    public async Task<ImmutableList<ECommerce>> MatchAllQueryAsync()
    {
        var result = await _client.SearchAsync<ECommerce>(s =>
            s.Index(indexName)
                .Size(100)
                    .Query(q => q
                        .MatchAll(m => { })));

        result = SetIds(result);

        return result.Documents.ToImmutableList();
    }

    public async Task<ImmutableList<ECommerce>> PaginationQueryAsync(int page, int pageSize)
    {
        var pageFrom = (page-1) * pageSize;

        var result = await _client.SearchAsync<ECommerce>(s =>
            s.Index(indexName)
                .Size(pageSize).From(pageFrom)
                    .Query(q => q
                        .MatchAll(m => { })));

        result = SetIds(result);

        return result.Documents.ToImmutableList();
    }

    private SearchResponse<ECommerce> SetIds(SearchResponse<ECommerce> result)
    {
        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;

        return result;
    }
}

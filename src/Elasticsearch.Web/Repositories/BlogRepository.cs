﻿using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Elasticsearch.Web.Models;

namespace Elasticsearch.Web.Repositories;

public class BlogRepository
{
    private readonly ElasticsearchClient _client;
    private const string indexName = "blog";

    public BlogRepository(ElasticsearchClient client)
    {
        _client = client;
    }

    public async Task<Blog?> SaveAsync(Blog newBlog)
    {
        newBlog.Created = DateTime.Now;
        var response = await _client.IndexAsync(newBlog, x => x.Index(indexName).Id(Guid.NewGuid().ToString()));

        if (!response.IsValidResponse)
        {
            return null;
        }

        newBlog.Id = response.Id;

        return newBlog;
    }

    public async Task<List<Blog>> SearchAsync(string searchText)
    {

        List<Action<QueryDescriptor<Blog>>> ListQuery = new();


        Action<QueryDescriptor<Blog>> matchAll = (q) => q.MatchAll(_ => { });

        Action<QueryDescriptor<Blog>> matchContent = (q) => q.Match(m => m
            .Field(f => f.Content)
            .Query(searchText));


        Action<QueryDescriptor<Blog>> titleMatchBoolPrefix = (q) => q.MatchBoolPrefix(m => m
            .Field(f => f.Content)
            .Query(searchText));


        Action<QueryDescriptor<Blog>> tagTerm = (q) => q.Term(t => t.Field(f => f.Tags).Value(searchText));


        if (string.IsNullOrEmpty(searchText))
        {
            ListQuery.Add(matchAll);
        }

        else
        { 
            ListQuery.Add(matchContent);
            ListQuery.Add(matchContent);
            ListQuery.Add(tagTerm);
        }

        var result = await _client.SearchAsync<Blog>(s => s.Index(indexName)
            .Size(1000).Query(q => q
                .Bool(b => b
                    .Should(ListQuery.ToArray()))));

        foreach (var hit in result.Hits) hit.Source.Id = hit.Id;
        return result.Documents.ToList();

    }
}

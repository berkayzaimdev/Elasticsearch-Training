using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Elasticsearch.Net;


namespace Elasticsearch.API.Extensions;

public static class ElasticsearchExtensions
{
    private const string Key = "Elastic";
    private const string V = "Url";

    public static void AddElastic(this IServiceCollection services, IConfiguration configuration)
    {
        var userName = configuration.GetSection("Elastic")["Username"]!.ToString();
        var password = configuration.GetSection("Elastic")["Password"]!.ToString();

        var settings = new ElasticsearchClientSettings(new Uri(uriString: configuration.GetSection(Key)![V]!)).Authentication(new BasicAuthentication(userName, password));

        var client = new ElasticsearchClient(settings);

        //var pool = new SingleNodeConnectionPool(new Uri(uriString: configuration.GetSection(Key)![V]!));
        //var settings = new ConnectionSettings(pool);
        //var client = new ElasticClient(settings);
        services.AddSingleton(client);
    }
}

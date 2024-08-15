using Elastic.Clients.Elasticsearch;
using Elastic.Transport;

namespace Elasticsearch.Web.Extensions;

public static class ElasticsearchExtensions
{
    private const string Key = "Elastic";
    private const string V = "Url";

    public static void AddElasticsearch(this IServiceCollection services, IConfiguration configuration)
    {
        var userName = configuration.GetSection("Elastic")["Username"]!.ToString();
        var password = configuration.GetSection("Elastic")["Password"]!.ToString();

        var settings = new ElasticsearchClientSettings(new Uri(uriString: configuration.GetSection(Key)![V]!)).Authentication(new BasicAuthentication(userName, password));

        var client = new ElasticsearchClient(settings);

        services.AddSingleton(client);
    }
}

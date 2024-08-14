using Elasticsearch.API.Models;

namespace Elasticsearch.API.Dtos;

public record ProductDto(string Id, string Name, decimal Price, int Stock, ProductFeatureDto? Feature)
{
}

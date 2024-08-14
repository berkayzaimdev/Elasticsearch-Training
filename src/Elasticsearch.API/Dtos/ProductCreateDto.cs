namespace Elasticsearch.API.Dtos;

public record ProductCreateDto(string Name, decimal Price, int Stock, ProductFeatureDto Feature);

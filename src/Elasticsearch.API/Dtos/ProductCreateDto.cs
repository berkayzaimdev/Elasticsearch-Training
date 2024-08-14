using Elasticsearch.API.Models;

namespace Elasticsearch.API.Dtos;

public record ProductCreateDto(int Width, int Height, EColor Color);

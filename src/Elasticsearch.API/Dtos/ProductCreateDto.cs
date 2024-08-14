using Elasticsearch.API.Models;

namespace Elasticsearch.API.Dtos;

public record ProductCreateDto(string Name, decimal Price, int Stock, ProductFeatureDto Feature)
{
    public Product CreateProduct()
    {
        return new Product
        {
            Name = Name,
            Price = Price,
            Stock = Stock,
            Feature = new ProductFeature()
            {
                Width = Feature.Width,
                Height = Feature.Height,
                Color = (EColor)int.Parse(Feature.Color)
            }
        };
    }
}

public record ProductUpdateDto(string Id, string Name, decimal Price, int Stock, ProductFeatureDto Feature)
{
    public Product CreateProduct()
    {
        return new Product
        {
            Name = Name,
            Price = Price,
            Stock = Stock,
            Feature = new ProductFeature()
            {
                Width = Feature.Width,
                Height = Feature.Height,
                Color = (EColor)int.Parse(Feature.Color)
            }
        };
    }
}


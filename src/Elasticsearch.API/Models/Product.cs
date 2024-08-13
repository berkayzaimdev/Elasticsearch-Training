using Nest;

namespace Elasticsearch.API.Models;

public class Product
{
    // Category => Product => ProductFeature
    [PropertyName("_id")]
    public string Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public ProductFeature? Feature { get; set; }
}

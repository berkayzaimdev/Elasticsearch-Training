using System.Text.Json.Serialization;

namespace Elasticsearch.Web.Models;

public class Blog
{
    [JsonPropertyName("_id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("title")]
    public string Title { get; set; } = default!;

    [JsonPropertyName("content")]
    public string Content { get; set; } = default!;

    [JsonPropertyName("tags")]
    public string[] Tags { get; set; } = default!;

    [JsonPropertyName("user_id")]
    public Guid UserId { get; set; } = default!;

    [JsonPropertyName("created")]
    public DateTime Created { get; set; } = default!;
}

namespace Elasticsearch.Web.ViewModels;

public class BlogCreateViewModel
{
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    public List<string> Tags { get; set; } = [];
}

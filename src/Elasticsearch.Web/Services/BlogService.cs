using Elasticsearch.Web.Models;
using Elasticsearch.Web.Repositories;
using Elasticsearch.Web.ViewModels;

namespace Elasticsearch.Web.Services;

public class BlogService
{
    private readonly BlogRepository _blogRepository;

    public BlogService(BlogRepository blogRepository)
    {
        _blogRepository = blogRepository;
    }

    public async Task<bool> SaveAsync(BlogCreateViewModel model)
    {
        Blog newBlog = new Blog()
        {
            Title = model.Title,
            Content = model.Content,
            Tags = model.Tags.ToArray(),
            UserId = Guid.NewGuid()
        };

        var response = await _blogRepository.SaveAsync(newBlog);

        return response is not null;
    }

    public async Task<List<BlogViewModel>> SearchAsync(string searchText)
    {
        var blogList = await _blogRepository.SearchAsync(searchText);

        return blogList.Select(
            b => new BlogViewModel() 
            {
                Id = b.Id,
                Title = b.Title,
                Content = b.Content
            }
        ).ToList();
    }
}
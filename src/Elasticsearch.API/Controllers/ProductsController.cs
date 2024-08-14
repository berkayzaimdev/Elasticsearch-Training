using Elasticsearch.API.Dtos;
using Elasticsearch.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Elasticsearch.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpPost]
        public async Task<IActionResult> Save(ProductCreateDto request)
        {
            var response = await _productService.SaveAsync(request);

            return Ok(response);
        }
    }
}

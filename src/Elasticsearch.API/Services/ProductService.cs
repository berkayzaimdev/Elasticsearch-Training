using Elasticsearch.API.Dtos;
using Elasticsearch.API.Repository;
using System.Net;

namespace Elasticsearch.API.Services;

public class ProductService
{
    private readonly ProductRepository _productRepository;

    public ProductService(ProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ResponseDto<ProductDto>> SaveAsync(ProductCreateDto request)
    {
        var responeProduct = await _productRepository.SaveAsync(request.CreateProduct());

        if(responeProduct is null)
        {
            return ResponseDto<ProductDto>.Fail(new List<string> { "Bir hata meydana geldi!" },
                                                System.Net.HttpStatusCode.InternalServerError);
        }

        return ResponseDto<ProductDto>.Success(responeProduct.CreateDto(), HttpStatusCode.Created);
    }
}

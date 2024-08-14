using Elasticsearch.API.Dtos;
using Elasticsearch.API.Repository;
using System.Collections.Generic;
using System.Collections.Immutable;
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

        if (responeProduct is null)
        {
            return ResponseDto<ProductDto>.Fail(new List<string> { "Bir hata meydana geldi!" },
                                                System.Net.HttpStatusCode.InternalServerError);
        }

        return ResponseDto<ProductDto>.Success(responeProduct.CreateDto(), HttpStatusCode.Created);
    }

    public async Task<ResponseDto<List<ProductDto>>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();

        var productListDto = products.Select(x => new ProductDto(
            x.Id,
            x.Name,
            x.Price,
            x.Stock,
            new ProductFeatureDto(x.Feature!.Width, x.Feature.Height, x.Feature.Color.ToString()))).ToList();

        return ResponseDto<List<ProductDto>>.Success(productListDto, HttpStatusCode.OK);
    }

    public async Task<ResponseDto<ProductDto>> GetByIdAsync(string id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null) return ResponseDto<ProductDto>.Fail("Product bulunamadı!", HttpStatusCode.NotFound);

        var productDto = product.CreateDto();

        return ResponseDto<ProductDto>.Success(productDto, HttpStatusCode.OK);
    }

    public async Task<ResponseDto<bool>> UpdateAsync(ProductUpdateDto updateProduct)
    {
        var isSuccess = await _productRepository.UpdateAsync(updateProduct);

        if(!isSuccess)
        {
            return ResponseDto<bool>.Fail("Güncelleme işlemi sırasında bir hata meydana geldi!", HttpStatusCode.InternalServerError);
        }

        return ResponseDto<bool>.Success(true, HttpStatusCode.NoContent);
    }
}

﻿using Elastic.Clients.Elasticsearch;
using Elastic.Transport.Products.Elasticsearch;
using Elasticsearch.API.Dtos;
using Elasticsearch.API.Repository;
using System.Net;

namespace Elasticsearch.API.Services;

public class ProductService
{
    private readonly ProductRepository _productRepository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(ProductRepository productRepository, ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
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

    public async Task<ResponseDto<bool>> DeleteAsync(string id)
    {
        var deleteResponse = await _productRepository.DeleteAsync(id);

        if (!deleteResponse.IsSuccess() && deleteResponse.Result == Result.NotFound)
        {
            return ResponseDto<bool>.Fail("Silme işlemi için belirtilen ID'ye sahip ürün bulunamadı!", HttpStatusCode.NotFound);
        }

        if (!deleteResponse.IsSuccess())
        {
            deleteResponse.TryGetOriginalException(out var exception);
            deleteResponse.TryGetElasticsearchServerError(out var error);
            _logger.LogError(exception, error.ToString());

            return ResponseDto<bool>.Fail("Silme işlemi sırasında bir hata meydana geldi!", HttpStatusCode.InternalServerError);
        }

        return ResponseDto<bool>.Success(true, HttpStatusCode.NoContent);
    }
}


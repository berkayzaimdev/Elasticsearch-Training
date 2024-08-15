using Elasticsearch.API.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Elasticsearch.API.Controllers;

public class ECommercesController : BaseController
{
    private readonly ECommerceRepository _eCommerceRepository;

    public ECommercesController(ECommerceRepository eCommerceRepository)
    {
        _eCommerceRepository = eCommerceRepository;
    }

    [HttpGet]
    public async Task<IActionResult> TermQuery([FromQuery] string firstName)
    {
        var values = await _eCommerceRepository.TermQueryAsync(firstName);
        return Ok(values);
    }

    [HttpPost]
    public async Task<IActionResult> TermsQuery([FromBody] List<string> firstNames)
    {
        var values = await _eCommerceRepository.TermsQueryAsync(firstNames);
        return Ok(values);
    }

    [HttpGet]
    public async Task<IActionResult> PrefixQuery(string fullName)
    {
        var values = await _eCommerceRepository.PrefixQueryAsync(fullName);
        return Ok(values);
    }

    [HttpGet]
    public async Task<IActionResult> RangeQuery(double fromPrice, double toPrice)
    {
        var values = await _eCommerceRepository.RangeQueryAsync(fromPrice, toPrice);
        return Ok(values);
    }

    [HttpGet]
    public async Task<IActionResult> MatchAllQuery()
    {
        var values = await _eCommerceRepository.MatchAllQueryAsync();
        return Ok(values);
    }

    [HttpGet]
    public async Task<IActionResult> PaginationQuery(int page = 1, int pageSize = 20)
    {
        var values = await _eCommerceRepository.PaginationQueryAsync(page, pageSize);
        return Ok(values);
    }

    [HttpGet]
    public async Task<IActionResult> WildcardQuery(string customerFullName)
    {
        var values = await _eCommerceRepository.WildcardQueryAsync(customerFullName);
        return Ok(values);
    }

    [HttpGet]
    public async Task<IActionResult> FuzzyQuery(string customerFirstName)
    {
        var values = await _eCommerceRepository.FuzzyQueryAsync(customerFirstName);
        return Ok(values);
    }

    [HttpGet]
    public async Task<IActionResult> MatchQueryFullText(string categoryName)
    {
        var values = await _eCommerceRepository.MatchQueryFullTextAsync(categoryName);
        return Ok(values);
    }

    [HttpGet]
    public async Task<IActionResult> MatchBoolPrefixQueryFullText(string customerFullName)
    {
        var values = await _eCommerceRepository.MatchBoolPrefixQueryFullTextAsync(customerFullName);
        return Ok(values);
    }

    [HttpGet]
    public async Task<IActionResult> MatchPhraseQueryFullText(string customerFullName)
    {
        var values = await _eCommerceRepository.MatchPhraseQueryFullTextAsync(customerFullName);
        return Ok(values);
    }

    [HttpGet]
    public async Task<IActionResult> CompoundQueryExampleOne(string cityName, double taxfulTotalPrice, string categoryName, string manufacturer)
    {
        var values = await _eCommerceRepository.CompoundQueryExampleOneAsync(cityName, taxfulTotalPrice, categoryName, manufacturer);
        return Ok(values);
    }

    [HttpGet]
    public async Task<IActionResult> CompoundQueryExampleTwo(string customerFullName)
    {
        var values = await _eCommerceRepository.CompoundQueryExampleTwoAsync(customerFullName);
        return Ok(values);
    }
}

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
        var values = await _eCommerceRepository.TermQuery(firstName);
        return Ok(values);
    }

    [HttpPost]
    public async Task<IActionResult> TermsQuery([FromBody] List<string> firstNames)
    {
        var values = await _eCommerceRepository.TermsQuery(firstNames);
        return Ok(values);
    }

    [HttpGet]
    public async Task<IActionResult> TermsQuery(string fullName)
    {
        var values = await _eCommerceRepository.PrefixQuery(fullName);
        return Ok(values);
    }
}

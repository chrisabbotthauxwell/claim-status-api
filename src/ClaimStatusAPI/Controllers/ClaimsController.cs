using Microsoft.AspNetCore.Mvc;
using ClaimStatusAPI.Models;
using ClaimStatusAPI.Services;
using System.Data;

namespace ClaimStatusAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ClaimsController : ControllerBase
{
    private readonly IClaimsService _claimsService;
    private readonly ILogger<ClaimsController> _logger;

    public ClaimsController(IClaimsService claimsService, ILogger<ClaimsController> logger)
    {
        _claimsService = claimsService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public ActionResult<Claim> GetById(string id)
    {
        _logger.LogInformation("Fetching claim with ID: {ClaimId}", id);
        var claim = _claimsService.GetById(id);
        if (claim is null)
        {
            _logger.LogWarning("Claim with ID: {ClaimId} not found", id);
            return NotFound();
        }
        _logger.LogInformation("Claim with ID: {ClaimId} found", id);
        return Ok(claim);
    }

    [HttpPost("{id}/summarize")]
    public async Task<ActionResult<ClaimSummary>> SummarizeClaimAsync(string id)
    {
        _logger.LogInformation("Summarizing claim with ID: {ClaimId}", id);
        var summary = await _claimsService.GetSummaryByIdAsync(id);
        if (summary is null)
        {
            _logger.LogWarning("Claim summary for ID: {ClaimId} not found", id);
            return NotFound();
        }
        _logger.LogInformation("Claim summary for ID: {ClaimId} generated", id);
        return Ok(summary);
    }
}
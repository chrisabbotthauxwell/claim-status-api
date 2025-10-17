using ClaimStatusAPI.Models;

namespace ClaimStatusAPI.Services;

public class ClaimsService : IClaimsService
{
    private readonly ILogger<ClaimsService> _logger;
    private readonly List<Claim> _claims;
    public ClaimsService(ILogger<ClaimsService> logger)
    {
        _logger = logger;

        var path = Path.Combine(AppContext.BaseDirectory, "mocks", "claims.json");
        if (File.Exists(path))
        {
            try
            {
                var json = File.ReadAllText(path);
                var options = new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                _claims = System.Text.Json.JsonSerializer.Deserialize<List<Claim>>(json, options) ?? new List<Claim>();
                _logger.LogInformation("Loaded {Count} claims from claims.json", _claims.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read/deserialize claims.json at {Path}", path);
                _claims = new List<Claim>();
            }
        }
        else
        {
            _logger.LogWarning("claims.json not found at {Path}", path);
            _claims = new List<Claim>();
        }
    }

    /// <summary>
    /// Get all claims
    /// </summary>
    /// <returns><see cref="IEnumerable"/><Claim></returns>
    public IEnumerable<Claim> GetAll()
    {
        _logger.LogInformation("Getting all claims");
        return _claims;
    }
    
    /// <summary>
    /// Get claim by id
    /// </summary>
    /// <param name="claimId"></param>
    /// <returns><see cref="Claim"/></returns>
    public Claim? GetById(string claimId)
    {
        _logger.LogInformation("Getting claim by id: {ClaimId}", claimId);
        ///var claim = _claims.FirstOrDefault(c => c.Id.Equals(claimId, StringComparison.OrdinalIgnoreCase));
        
        
        if (_claims == null || _claims.Count == 0)
        {
            _logger.LogWarning("No claims loaded to search for id: {ClaimId}", claimId);
            return null;
        }

        var claim = _claims.FirstOrDefault(c => c?.Id != null && c.Id.Equals(claimId, StringComparison.OrdinalIgnoreCase));

        if (claim is null)
        {
            _logger.LogWarning("Claim with id: {ClaimId} not found", claimId);
        }
        else
        {
            _logger.LogInformation("Claim with id: {ClaimId} found", claimId);
        }
        return claim;
    }
}
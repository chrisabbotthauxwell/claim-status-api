using ClaimStatusAPI.Models;
using OpenAI.Chat;
using System.Text.Json;

namespace ClaimStatusAPI.Services;

public class ClaimsService : IClaimsService
{
    private readonly ILogger<ClaimsService> _logger;
    private readonly List<Claim> _claims = new List<Claim>();
    private readonly List<ClaimsNotes> _claimsNotes = new List<ClaimsNotes>();
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    private readonly IOpenAiService _openAiService;
    public ClaimsService(ILogger<ClaimsService> logger)
    {
        _logger = logger;

        // Load claims.json
        var claimsPath = Path.Combine(AppContext.BaseDirectory, "mocks", "claims.json");
        if (File.Exists(claimsPath))
        {
            try
            {
                var json = File.ReadAllText(claimsPath);
                _claims = JsonSerializer.Deserialize<List<Claim>>(json, _jsonOptions) ?? new List<Claim>();
                _logger.LogInformation("Loaded {Count} claims from claims.json", _claims.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read/deserialize claims.json at {Path}", claimsPath);
            }
        }
        else
        {
            _logger.LogWarning("claims.json not found at {Path}", claimsPath);
        }

        // Load notes.json
        var notesPath = Path.Combine(AppContext.BaseDirectory, "mocks", "notes.json");
        _claimsNotes = null;
        if (File.Exists(notesPath))
        {
            try
            {
                var notesJson = File.ReadAllText(notesPath);
                _claimsNotes = JsonSerializer.Deserialize<List<ClaimsNotes>>(notesJson, _jsonOptions) ?? new List<ClaimsNotes>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to read/deserialize notes.json at {Path}", notesPath);
            }
        }
        else
        {
            _logger.LogWarning("notes.json not found at {Path}", notesPath);
        }

    }

    public ClaimsService (ILogger<ClaimsService> logger, IOpenAiService openAiService) : this(logger)
    {
        _openAiService = openAiService;
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
    
    public async Task<ClaimSummary?> GetSummaryByIdAsync(string claimId)
    {
        _logger.LogInformation("Getting claim summary by id: {ClaimId}", claimId);

        var claim = GetById(claimId);
        if (claim is null)
        {
            _logger.LogWarning("Claim summary for id: {ClaimId} not found", claimId);
            return null;
        }

        if (_openAiService == null)
        {
            _logger.LogError("OpenAI service not available - cannot generate summary for {ClaimId}", claimId);
            return null;
        }

        // Read the notes for the claim
        var claimNotes = _claimsNotes.FirstOrDefault(n => n?.Id != null && n.Id.Equals(claimId, StringComparison.OrdinalIgnoreCase))?.Notes;

        // Build messages for chat completion
        //var systemPrompt = new ChatMessage(ChatRole.System, "You are a claims summarization assistant. Produce a valid JSON object with keys: summary, customerSummary, adjusterSummary, nextStep. Return ONLY the JSON object.");
        var userPayload = new
        {
            claim,
            notes = claimNotes
        };
        var userContent = JsonSerializer.Serialize(userPayload, new JsonSerializerOptions { WriteIndented = false, PropertyNamingPolicy = null });

        //var userMessage = new ChatMessage(ChatRole.User, $"Please generate claim summaries from the following data (claim + notes) and return strict JSON:\n\n{userContent}");

        //var messages = new List<ChatMessage> { systemPrompt, userMessage };

        var messages = new List<ChatMessage>
        {
            new SystemChatMessage(@"You are a claims summarization assistant. Produce a valid JSON object with keys: summary, customerSummary, adjusterSummary, nextStep. Return ONLY the JSON object."),
            new UserChatMessage(userContent),
        };

        // Call OpenAI to get the summary
        try
        {
            var completion = await _openAiService.CreateChatCompletionAsync(messages);

            if (completion is null)
            {
                _logger.LogWarning("OpenAI returned null response for claim {ClaimId}", claimId);
                return null;
            }

            // Parse response into ClaimSummary
            try
            {
                var summary = JsonSerializer.Deserialize<ClaimSummary>(completion.Content[0].Text, _jsonOptions);
                if (summary == null)
                {
                    _logger.LogWarning("OpenAI response could not be parsed into ClaimSummary for {ClaimId}. Response: {Response}", claimId, completion);
                }
                else
                {
                    _logger.LogInformation("Generated ClaimSummary for {ClaimId}", claimId);
                }
                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to parse OpenAI response for {ClaimId}. Response: {Response}", claimId, completion);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OpenAI call failed for {ClaimId}", claimId);
            return null;
        }
    }
}
using ClaimStatusAPI.Models;
using System.Threading.Tasks;

namespace ClaimStatusAPI.Services;
public interface IClaimsService
{
    Claim? GetById(string claimId);

    // Returns a multi-part summary for the claim, or null if not found
    Task<ClaimSummary?> GetSummaryByIdAsync(string claimId);
}
using ClaimStatusAPI.Models;

namespace ClaimStatusAPI.Services;
public interface IClaimsService
{
    Claim? GetById(string claimId);
}
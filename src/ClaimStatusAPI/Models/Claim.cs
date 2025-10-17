namespace ClaimStatusAPI.Models;

public class Claim
{
    public string Id { get; set; }
    public string ClaimNumber { get; set; }
    public string PolicyNumber { get; set; }
    public Claimant Claimant { get; set; }
    public DateTime DateOfLoss { get; set; }
    public DateTime ReportedDate { get; set; }
    public string Status { get; set; }
    public decimal ReserveAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal EstimatedLoss { get; set; }
    public string AdjusterId { get; set; }
    public string Description { get; set; }
    public string NotesRef { get; set; }
}
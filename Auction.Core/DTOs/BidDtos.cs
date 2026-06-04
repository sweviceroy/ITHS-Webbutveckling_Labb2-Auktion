using System.ComponentModel.DataAnnotations;

namespace Auction.Core.DTOs;

public class CreateBidDto
{
    [Required]
    public decimal Amount { get; set; }
}

public class BidDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime BidTime { get; set; }
    public string BidderUsername { get; set; } = string.Empty;
    public Guid BidderId { get; set; }
}

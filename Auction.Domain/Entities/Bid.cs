using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Auction.Domain.Entities;

public class Bid
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, Column(TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    public DateTime BidTime { get; set; } = DateTime.UtcNow;

    [Required]
    public Guid AuctionId { get; set; }

    [ForeignKey(nameof(AuctionId))]
    public Auction Auction { get; set; } = null!;

    [Required]
    public Guid BidderId { get; set; }

    [ForeignKey(nameof(BidderId))]
    public User Bidder { get; set; } = null!;
}

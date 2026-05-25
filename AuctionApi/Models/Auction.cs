using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuctionApi.Models;

public class Auction
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [Required, Column(TypeName = "decimal(18,2)")]
    public decimal StartingPrice { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? CurrentHighestBid { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; } = true;

    [Required]
    public Guid CreatorId { get; set; }

    [ForeignKey(nameof(CreatorId))]
    public User Creator { get; set; } = null!;

    public ICollection<Bid> Bids { get; set; } = new List<Bid>();
}

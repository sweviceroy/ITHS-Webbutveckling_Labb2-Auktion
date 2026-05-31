using System.ComponentModel.DataAnnotations;

namespace AuctionApi.DTOs;

public class CreateAuctionDto
{
    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    [Required]
    public decimal StartingPrice { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }
}

public class UpdateAuctionDto
{
    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public decimal? StartingPrice { get; set; }

    [Required]
    public DateTime EndDate { get; set; }
}

public class AuctionListDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal StartingPrice { get; set; }
    public decimal? CurrentHighestBid { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsOpen { get; set; }
    public int BidCount { get; set; }
    public string CreatorUsername { get; set; } = string.Empty;
}

public class AuctionDetailDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
    public decimal StartingPrice { get; set; }
    public decimal? CurrentHighestBid { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsOpen { get; set; }
    public Guid CreatorId { get; set; }
    public string CreatorUsername { get; set; } = string.Empty;
    public List<BidDto> Bids { get; set; } = new();
}

using Auction.Domain.Entities;

namespace Auction.Data;

public static class DbSeeder
{
    public static void Seed(AuctionDbContext db)
    {
        if (db.Users.Any())
            return;

        var admin = new User
        {
            Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Username = "admin",
            Email = "admin@auction.se",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            IsAdmin = true
        };

        var user1 = new User
        {
            Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Username = "erik",
            Email = "erik@email.se",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("erik123")
        };

        var user2 = new User
        {
            Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
            Username = "lisa",
            Email = "lisa@email.se",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("lisa123")
        };

        db.Users.AddRange(admin, user1, user2);

        var auction1 = new AuctionEntity
        {
            Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
            Title = "Vintage Bicycle",
            Description = "A beautiful vintage bicycle from the 1960s in excellent condition.",
            ImageUrl = "https://images.unsplash.com/photo-1507035895480-2b3156c31fc8?w=500",
            StartingPrice = 500,
            StartDate = DateTime.UtcNow.AddDays(-5),
            EndDate = DateTime.UtcNow.AddDays(5),
            CreatorId = user1.Id
        };

        var auction2 = new AuctionEntity
        {
            Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
            Title = "Antique Bookshelf",
            Description = "Solid oak bookshelf with intricate carvings. A true statement piece.",
            ImageUrl = "https://images.unsplash.com/photo-1594620302200-9a762244a156?w=500",
            StartingPrice = 1200,
            StartDate = DateTime.UtcNow.AddDays(-3),
            EndDate = DateTime.UtcNow.AddDays(7),
            CreatorId = user1.Id
        };

        var auction3 = new AuctionEntity
        {
            Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
            Title = "Gaming Laptop",
            Description = "High-end gaming laptop, RTX 4080, 32GB RAM, barely used.",
            ImageUrl = "https://images.unsplash.com/photo-1603302576837-37561b2e2302?w=500",
            StartingPrice = 8000,
            StartDate = DateTime.UtcNow.AddDays(-1),
            EndDate = DateTime.UtcNow.AddDays(3),
            CreatorId = user2.Id
        };

        var auction4 = new AuctionEntity
        {
            Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
            Title = "Handmade Pottery Set",
            Description = "Complete set of handmade ceramic plates, bowls and cups.",
            ImageUrl = "https://images.unsplash.com/photo-1565193566173-7a0ee3dbe261?w=500",
            StartingPrice = 300,
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(10),
            CreatorId = user2.Id
        };

        var auction5 = new AuctionEntity
        {
            Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
            Title = "Electric Guitar",
            Description = "Fender Stratocaster, sunburst finish. Perfect for beginners and pros.",
            ImageUrl = "https://images.unsplash.com/photo-1550985616-10810253b84d?w=500",
            StartingPrice = 3500,
            StartDate = DateTime.UtcNow.AddDays(-2),
            EndDate = DateTime.UtcNow.AddDays(2),
            CreatorId = user1.Id
        };

        db.Auctions.AddRange(auction1, auction2, auction3, auction4, auction5);

        auction1.CurrentHighestBid = 750;
        auction2.CurrentHighestBid = 1300;
        auction3.CurrentHighestBid = 9500;
        auction4.CurrentHighestBid = 450;
        auction5.CurrentHighestBid = 3750;

        db.Bids.AddRange(
            new Bid { Amount = 600, AuctionId = auction1.Id, BidderId = user2.Id, BidTime = DateTime.UtcNow.AddDays(-3) },
            new Bid { Amount = 750, AuctionId = auction1.Id, BidderId = user2.Id, BidTime = DateTime.UtcNow.AddDays(-2) },
            new Bid { Amount = 1300, AuctionId = auction2.Id, BidderId = user2.Id, BidTime = DateTime.UtcNow.AddDays(-1) },
            new Bid { Amount = 8500, AuctionId = auction3.Id, BidderId = user1.Id, BidTime = DateTime.UtcNow.AddHours(-12) },
            new Bid { Amount = 9500, AuctionId = auction3.Id, BidderId = user1.Id, BidTime = DateTime.UtcNow.AddHours(-6) },
            new Bid { Amount = 400, AuctionId = auction4.Id, BidderId = user1.Id, BidTime = DateTime.UtcNow.AddDays(-5) },
            new Bid { Amount = 450, AuctionId = auction4.Id, BidderId = user1.Id, BidTime = DateTime.UtcNow.AddDays(-3) },
            new Bid { Amount = 3750, AuctionId = auction5.Id, BidderId = user2.Id, BidTime = DateTime.UtcNow.AddHours(-2) }
        );

        db.SaveChanges();
    }
}

using MassTransit;
using MongoDB.Driver;
using CustomerService.Data;       

public class BookingCompletedEvent
{
    public string UserId { get; set; } = string.Empty;
    public string BookingId { get; set; } = string.Empty;
    public DateTime CompletedAt { get; set; }
}

public class BookingCompletedConsumer : IConsumer<BookingCompletedEvent>
{
    private readonly MongoDbContext _context;

    public BookingCompletedConsumer(MongoDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<BookingCompletedEvent> context)
    {
        var userId = context.Message.UserId;
        var user = await _context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();

        if (user == null) return;

        if (user.HasReceivedDiscount) return;

        user.BookingCount++;

        if (user.BookingCount > 3)
        {
            user.HasReceivedDiscount = true;
            user.Inbox.Add("Discount available!");
        }

        await _context.Users.ReplaceOneAsync(u => u.Id == userId, user);
    }
}

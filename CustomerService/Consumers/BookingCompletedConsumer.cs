using MassTransit;
using MongoDB.Driver;
using CustomerService.Data;
using Shared.Contracts;

public class BookingCompletedConsumer : IConsumer<BookingCompletedEvent>
{
    private readonly MongoDbContext _context;

    public BookingCompletedConsumer(MongoDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<BookingCompletedEvent> context)
    {
        Console.WriteLine("BookingCompletedConsumer hit");

        var userId = context.Message.UserId;
        Console.WriteLine($"Event for user: {userId}");

        var user = await _context.Users.Find(u => u.Id == userId).FirstOrDefaultAsync();

        if (user == null)
        {
            Console.WriteLine("User not found");
            return;
        }

        Console.WriteLine($"Found user: {user.Email}");

        user.BookingCount++;

        if (user.BookingCount == 3 && !user.HasReceivedDiscount)
        {
            user.HasReceivedDiscount = true;
            user.Inbox.Add("You've unlocked a discount on your next ride!");
            Console.WriteLine("Discount granted!");
        }

        await _context.Users.ReplaceOneAsync(u => u.Id == userId, user);
    }

}

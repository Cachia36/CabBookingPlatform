using MassTransit;
using MongoDB.Driver;
using CustomerService.Data;
using Shared.Contracts;
using CustomerService.Models;

public class BookingCompletedConsumer : IConsumer<BookingCompletedEvent>
{
    private readonly MongoDbContext _context;

    public BookingCompletedConsumer(MongoDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<BookingCompletedEvent> context)
    {
        Console.WriteLine("BookingCompletedEvent called");
        var user = await _context.Users.Find(u => u.Id == context.Message.UserId).FirstOrDefaultAsync();

        if (user == null)
        {
            Console.WriteLine($"User {context.Message.UserId} not found.");
            return;
        }
 
        user.BookingCount++;

        if (user.BookingCount == 3 && !user.HasReceivedDiscount)
        {
            user.HasReceivedDiscount = true;
            user.Inbox.Add("You've unlocked a discount on your next ride!");
            Console.WriteLine("Discount granted!");
        }

        await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user);
    }
}
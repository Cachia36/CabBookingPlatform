﻿using Shared.Contracts;
using MongoDB.Driver;
using MassTransit;
using CustomerService.Data;

namespace CustomerService.Consumers
{
    public class CabReadyConsumer : IConsumer<CabReadyEvent>
    {
        private readonly MongoDbContext _context;

        public CabReadyConsumer(MongoDbContext context)
        {
            _context = context;
        }

        public async Task Consume(ConsumeContext<CabReadyEvent> context)
        {
            Console.WriteLine("CabReadyEvent consumer called");
            var user = await _context.Users.Find(u => u.Id == context.Message.UserId).FirstOrDefaultAsync();
            if (user == null)
            {
                Console.WriteLine($"User not found for cab ready event: {context.Message.UserId}");
                return;
            }

            var booking = context.Message;

            var notification =
                $"Your cab is ready for pickup!\n" +
                $"Booking Details:\n" +
                $"- Pickup: {booking.StartLocation}\n" +
                $"- Dropoff: {booking.EndLocation}\n" +
                $"- Scheduled Time: {booking.DateTime}\n" +
                $"- Passengers: {booking.PassengerCount}";

            user.Inbox.Add(notification);

            await _context.Users.ReplaceOneAsync(u => u.Id == user.Id, user);
            Console.WriteLine($"Cab ready notification sent to user {user.Email}");
        }
    }


}

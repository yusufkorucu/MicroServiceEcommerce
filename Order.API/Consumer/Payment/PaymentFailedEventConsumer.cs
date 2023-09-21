using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Moduls;
using Order.API.Moduls.Enums;
using Shared.Events.Payment;

namespace Order.API.Consumer.Payment
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        readonly OrderAPIDbContext _orderAPIDbContext;

        public PaymentFailedEventConsumer(OrderAPIDbContext orderAPIDbContext)
        {
            _orderAPIDbContext = orderAPIDbContext;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            Order.API.Models.Entities.Order order = await _orderAPIDbContext.Orders.FirstOrDefaultAsync(x => x.OrderId == context.Message.OrderId);

            order.OrderStatus = OrderStatus.Failed;
            await _orderAPIDbContext.SaveChangesAsync();
        }
    }
}

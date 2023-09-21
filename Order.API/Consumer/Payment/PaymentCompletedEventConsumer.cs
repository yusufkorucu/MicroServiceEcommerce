using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Moduls;
using Order.API.Moduls.Enums;
using Shared.Events.Payment;

namespace Order.API.Consumer.Payment
{
    public class PaymentCompletedEventConsumer : IConsumer<PaymentCompletedEvent>
    {
        readonly OrderAPIDbContext _orderAPIDbContext;

        public PaymentCompletedEventConsumer(OrderAPIDbContext orderAPIDbContext)
        {
            _orderAPIDbContext = orderAPIDbContext;
        }

        public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
          Order.API.Models.Entities.Order order= await  _orderAPIDbContext.Orders.FirstOrDefaultAsync(x => x.OrderId == context.Message.OrderId);

            order.OrderStatus = OrderStatus.Completed;
            await _orderAPIDbContext.SaveChangesAsync();
        }
    }
}

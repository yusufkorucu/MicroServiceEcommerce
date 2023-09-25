using MassTransit;
using Order.API.Moduls.Enums;
using Order.API.Moduls;
using Microsoft.EntityFrameworkCore;
using Shared.Events.Invoice;

namespace Order.API.Consumer.Invoice
{
    public class InvoiceCreatedEventConsumer : IConsumer<InvoiceCreatedEvent>
    {
        readonly OrderAPIDbContext _orderAPIDbContext;

        public InvoiceCreatedEventConsumer(OrderAPIDbContext orderAPIDbContext)
        {
            _orderAPIDbContext = orderAPIDbContext;
        }

        public async Task Consume(ConsumeContext<InvoiceCreatedEvent> context)
        {
            Order.API.Models.Entities.Order order = await _orderAPIDbContext.Orders.FirstOrDefaultAsync(x => x.OrderId == context.Message.OrderId);

            order.OrderStatus = OrderStatus.Invoiced;
            await _orderAPIDbContext.SaveChangesAsync();
        }
    }
}

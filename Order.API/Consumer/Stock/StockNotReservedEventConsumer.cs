using MassTransit;
using Order.API.Moduls.Enums;
using Order.API.Moduls;
using Shared.Events.Stock;
using Microsoft.EntityFrameworkCore;

namespace Order.API.Consumer.Stock
{
    public class StockNotReservedEventConsumer : IConsumer<StockNotReservedEvent>
    {
        readonly OrderAPIDbContext _orderAPIDbContext;

        public StockNotReservedEventConsumer(OrderAPIDbContext orderAPIDbContext)
        {
            _orderAPIDbContext = orderAPIDbContext;
        }
        public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
        {
            Order.API.Models.Entities.Order order = await _orderAPIDbContext.Orders.FirstOrDefaultAsync(x => x.OrderId == context.Message.OrderId);

            order.OrderStatus = OrderStatus.Failed;
            await _orderAPIDbContext.SaveChangesAsync();
        }
    }
}

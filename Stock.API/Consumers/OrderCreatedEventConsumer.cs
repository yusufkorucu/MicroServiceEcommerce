using Amazon.Runtime;
using MassTransit;
using MongoDB.Driver;
using Shared;
using Shared.Events.Order;
using Shared.Events.Stock;
using Shared.Messages.Order;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {

        IMongoCollection<Stock.API.Models.Entities.Stock> _stockColletion;
        readonly ISendEndpointProvider _sendEndpointProvider;

        readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(IMongoCollection<Models.Entities.Stock> stockColletion, ISendEndpointProvider sendEndpointProvider = null, IPublishEndpoint publishEndpoint = null)
        {
            _stockColletion = stockColletion;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();

            foreach (OrderItemMessage orderItem in context.Message.OrderItems)
            {
                stockResult.Add((await _stockColletion.FindAsync(s => s.ProductId == orderItem.ProductId && s.Count >= orderItem.Count)).Any());

            }

            if (stockResult.TrueForAll(s => s.Equals(true)))
            {
                foreach (OrderItemMessage orderItem in context.Message.OrderItems)
                {
                    Stock.API.Models.Entities.Stock stock = await (await _stockColletion.FindAsync(s => s.ProductId == orderItem.ProductId)).FirstOrDefaultAsync();

                    stock.Count -= orderItem.Count;

                    await _stockColletion.FindOneAndReplaceAsync(s => s.ProductId == orderItem.ProductId, stock);

                    StockReservedEvent stockReservedEvent = new()
                    {
                        BuyerId = context.Message.BuyerId,
                        OrderId = context.Message.OrderId,
                        TotalPrice = context.Message.TotalPrice
                    };

                    ISendEndpoint sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.Payment_StockReservedEventQueue}"));

                    await sendEndpoint.Send(stockReservedEvent);
                }

                Console.WriteLine("Stok Başarılı");
            }
            else
            {
                // hata dönülecek yers

                StockNotReservedEvent stockNotReservedEvent = new()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    Message = "Stock Yetersiz"
                };

               await  _publishEndpoint.Publish(stockNotReservedEvent);

                Console.WriteLine("Stok Başarısız");
            }

            await Task.CompletedTask;
        }
    }
}

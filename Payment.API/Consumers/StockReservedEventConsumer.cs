using MassTransit;
using Shared.Events.Payment;
using Shared.Events.Stock;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        readonly IPublishEndpoint _publishEndpoint;

        public StockReservedEventConsumer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            // payment Process

            if (true)
            {
                PaymentCompletedEvent paymentCompletedEvent = new()
                {
                    OrderId = context.Message.OrderId
                };

                _publishEndpoint.Publish(paymentCompletedEvent);

                Console.WriteLine("Ödeme Başarılı");
            }
            else
            {
                PaymentFailedEvent paymentFailedEvent = new()
                {
                    OrderId = context.Message.OrderId,
                    Message = "Ödeme Servis Hata Mesajı"
                };
                _publishEndpoint.Publish(paymentFailedEvent);

                Console.WriteLine("Ödeme Başarısız");
            }
            return Task.CompletedTask;
        }
    }
}

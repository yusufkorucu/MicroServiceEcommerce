using MassTransit;
using Shared.Events.Invoice;
using Shared.Events.Payment;

namespace Inovice.API.Consumer
{
    public class PaymentCompetedEventConsumer : IConsumer<PaymentCompletedEvent>
    {
        readonly IPublishEndpoint _publishEndpoint;

        public PaymentCompetedEventConsumer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task Consume(ConsumeContext<PaymentCompletedEvent> context)
        {
            // logic koşar fatura işlermleri için sonra fatura kesilir Id değeri alınır

            InvoiceCreatedEvent invoiceCreatedEvent = new()
            {
                InvoiceId = Guid.NewGuid(),
                OrderId = context.Message.OrderId
            };

            _publishEndpoint.Publish(invoiceCreatedEvent);

            return Task.CompletedTask;
        }
    }
}

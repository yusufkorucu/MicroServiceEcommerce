using Shared.Event.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events.Invoice
{
    public class InvoiceCreatedEvent : IEvent
    {
        public Guid OrderId { get; set; }
        public Guid InvoiceId { get; set; }

    }

}

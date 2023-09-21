using Shared.Event.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events.Payment
{
    public class PaymentCompletedEvent : IEvent
    {
        public Guid OrderId { get; set; }
    }
}

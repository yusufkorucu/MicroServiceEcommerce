using Shared.Event.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events.Stock
{
    public class StockNotReservedEvent:IEvent
    {
        public Guid BuyerId { get; set; }
        public Guid OrderId { get; set; }
        public string Message { get; set; }
    }
}

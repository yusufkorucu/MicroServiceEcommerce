namespace Order.API.ViewModels
{
    public class CreateOrderItemVM
    {
        public Guid ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}

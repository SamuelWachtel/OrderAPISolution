namespace OrderApi.Orders
{
    public class Order
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderCreationDate { get; set; }
        public OrderStatus Status { get; set; }
        public OrderItems OrderItems { get; set; }
    }

    public class OrderItems
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; }
    }
}

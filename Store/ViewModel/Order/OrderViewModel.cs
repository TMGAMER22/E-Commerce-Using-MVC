namespace Store.ViewModel.Order
{
    public class OrderViewModel
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Notes { get; set; }
        public List<OrderItemViewModel> Items { get; set; }
    }
}

namespace Store.Models
{
    public class Cart
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public List<CartItem> Items { get; set; }
    }
}

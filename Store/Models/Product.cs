using System.ComponentModel;

namespace Store.Models
{
  public class Product
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public Guid? CategoryId { get; set; }
        public Category? Category { get; set; }
        public string? ImagePath { get; set; }
        public string? UserId { get; set; } 
        public ApplicationUser? User { get; set; }
    }  
}

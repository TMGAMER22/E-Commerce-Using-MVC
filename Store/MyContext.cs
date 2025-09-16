using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Store.Chat.Models;
using Store.Models;
using System.Reflection.Emit;

namespace Store
{
    public class MyContext :IdentityDbContext
    {
        public DbSet<Product> products {  get; set; }
        public DbSet<Category> categories { get; set; }
        public DbSet<Cart> carts { get; set; }
        public DbSet<CartItem> cartItems { get; set; }
        public DbSet <Order> orders { get; set; }
        public DbSet<OrderItems> ordersItems { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }


        public MyContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

               builder.Entity<IdentityRole>().HasData(
                    new IdentityRole 
                    { 
                        Id = Guid.Parse("b7b6c2c1-5d2b-4c91-8f1a-88f556a04b44").ToString(),
                        Name = "Admin", NormalizedName = "ADMIN",
                        ConcurrencyStamp = "11111111-aaaa-1111-aaaa-111111111111"
                    },
                    new IdentityRole 
                    { 
                        Id = Guid.Parse("a2f43d77-9b8c-4c1f-b1a4-7f8f6f3f2345").ToString(),
                        Name = "Company", NormalizedName = "COMPANY",
                        ConcurrencyStamp = "22222222-bbbb-2222-bbbb-222222222222"
                    },
                    new IdentityRole 
                    { 
                        Id = Guid.Parse("c4f43d77-9b8c-4c1f-b1a4-7f8f6f3f6789").ToString(),
                        Name = "Customer", NormalizedName = "CUSTOMER",
                        ConcurrencyStamp = "33333333-cccc-3333-cccc-333333333333"
                    }
                );

               builder.Entity<Category>().HasData(
                    new Category
                    {
                        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                        Name = "Electronics"
                    },
                    new Category
                    {
                        Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                        Name = "MedKits"
                    },
                    new Category
                    {
                        Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                        Name = "Books"
                    },
                    new Category
                    {
                        Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                        Name = "SeaFood"
                    },
                    new Category
                    {
                        Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                        Name = "Figures"
                    }
                );

            builder.Entity<Product>()
                .Property(p => p.Name)
                .IsRequired();

            builder.Entity<Category>()
                .Property(p => p.Name)
                .HasMaxLength(100);

            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            builder.Entity<Conversation>()
                .HasOne(c => c.Client)
                .WithMany()
                .HasForeignKey(c => c.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Conversation>()
                .HasOne(c => c.Company)
                .WithMany()
                .HasForeignKey(c => c.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}

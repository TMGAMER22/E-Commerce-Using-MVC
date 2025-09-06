using Microsoft.AspNetCore.Identity;
using Store;
using Store.Models;
using Store.Repositories.Products;
using Microsoft.EntityFrameworkCore;

public class ProductRepository : IProductRepository
{
    private readonly MyContext context;
    private readonly UserManager<ApplicationUser> userManager;

    public ProductRepository(MyContext _context, UserManager<ApplicationUser> userManager)
    {
        context = _context;
        this.userManager = userManager;
    }

    public async Task<List<Product>> GetAll()
    {
        return await context.products.ToListAsync();
    }

    public async Task<List<Product>> GetAllWithCompany()
    {
        return await context.products
                            .Include(p => p.User)
                            .Include(p => p.Category)
                            .ToListAsync();
    }

    public async Task<Product?> GetById(Guid id)
    {
        return await context.products.FindAsync(id);
    }

    public async Task<List<Product>> GetByUserId(string userId)
    {
        return await context.products
                            .Where(p => p.UserId == userId)
                            .ToListAsync();
    }

    public async Task<List<Product>> GetByCategoryId(Guid categoryId)
    {
        return await context.products
                            .Where(p => p.CategoryId == categoryId)
                            .ToListAsync();
    }

    public void Add(Product product) => context.products.Add(product);

    public void Update(Product product) => context.products.Update(product);

    public void Delete(Guid id)
    {
        var product = context.products.Find(id);
        if (product != null)
            context.products.Remove(product);
    }

    public async Task Save() => await context.SaveChangesAsync();

    public async Task<Cart?> GetCartByUserId(string userId)
    {
        return await context.carts
                            .Include(c => c.Items)
                            .ThenInclude(i => i.Product)
                            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

   
}

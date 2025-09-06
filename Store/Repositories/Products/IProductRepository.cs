using Store.Models;

namespace Store.Repositories.Products
{
    public interface IProductRepository
    {
        Task<List<Product>> GetAll();
        Task<List<Product>> GetAllWithCompany();
        Task<Product> GetById(Guid id);
        Task<List<Product>> GetByUserId(string UserId);
        Task<List<Product>> GetByCategoryId(Guid categoryId);
        Task<Cart?> GetCartByUserId(string userId);
        void Add(Product product);
        void Update(Product product);
        void Delete(Guid id);
        Task Save();   
    }
}

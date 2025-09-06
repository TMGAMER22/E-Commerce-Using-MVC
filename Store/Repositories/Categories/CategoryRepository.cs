
namespace Store.Repositories.Categories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly MyContext context;

        public CategoryRepository(MyContext _context)
        {
            context = _context;
        }
        public List<Models.Category> GetAll()
        {
            return context.categories.ToList();
        }
    }
}

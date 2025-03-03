using Domain;

namespace DAL;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public List<ItemCategory> GetCategories()
    {
        return _context.ItemCategories.ToList();
    }

    public ItemCategory? GetCategoryById(int id)
    {
        return _context.ItemCategories.FirstOrDefault(c => c.Id == id);
    }

    public bool AddCategory(ItemCategory category)
    {
        try
        {
            _context.ItemCategories.Add(category);
            _context.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
}
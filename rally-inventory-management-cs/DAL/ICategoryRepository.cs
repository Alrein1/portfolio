using Domain;

namespace DAL;

public interface ICategoryRepository
{
    List<ItemCategory> GetCategories();
    ItemCategory? GetCategoryById(int id);
    bool AddCategory(ItemCategory category);
}
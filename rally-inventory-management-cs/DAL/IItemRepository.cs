using Domain;

namespace DAL;

public interface IItemRepository
{
    List<Item> GetItems();
    Item? GetItemById(int id);
    bool AddItem(Item item);
    bool UpdateItem(Item item);
}
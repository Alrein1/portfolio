using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class UsedInventoryModel : PageModel
{
    private readonly IItemRepository _itemRepository;

    public UsedInventoryModel(IItemRepository itemRepository)
    {
        _itemRepository = itemRepository;
    }

    public List<Item>? ItemsToRestock { get; set; }
    public double TotalRestockCost { get; set; }

    public void OnGet()
    {
        var allItems = _itemRepository.GetItems();
        ItemsToRestock = allItems.Where(i => i.Quantity < i.OptimalQuantity).ToList();
        TotalRestockCost = ItemsToRestock.Sum(i => (i.OptimalQuantity - i.Quantity) * i.Price);
    }
}
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class InventoryModel : PageModel
{
    private readonly IItemRepository _itemRepository;
    private readonly ICategoryRepository _categoryRepository;

    public InventoryModel(IItemRepository itemRepository, ICategoryRepository categoryRepository)
    {
        _itemRepository = itemRepository;
        _categoryRepository = categoryRepository;
    }

    public List<Item>? Items { get; set; }
    public List<ItemCategory>? Categories { get; set; }
    [BindProperty(SupportsGet = true)]
    public string? SearchQuery { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public int? CategoryId { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? QuantityFilter { get; set; }


    public void OnGet()
    {
        Categories = _categoryRepository.GetCategories();
        var allItems = _itemRepository.GetItems();
        var filteredItems = allItems;

        if (CategoryId.HasValue && CategoryId.Value != 0)
        {
            filteredItems = filteredItems.Where(i => i.CategoryId == CategoryId.Value).ToList();
        }

        if (!string.IsNullOrWhiteSpace(QuantityFilter))
        {
            filteredItems = QuantityFilter.ToLower() switch
            {
                "below" => filteredItems.Where(i => i.Quantity < i.OptimalQuantity).ToList(),
                "above" => filteredItems.Where(i => i.Quantity > i.OptimalQuantity).ToList(),
                "zero" => filteredItems.Where(i => i.Quantity == 0).ToList(),
                _ => filteredItems
            };
        }

        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var searchTerms = SearchQuery.Split(',')
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .ToList();

            var inclusionTerms = searchTerms
                .Where(t => !t.StartsWith("!"))
                .Select(t => t.ToLower())
                .ToList();

            var exclusionTerms = searchTerms
                .Where(t => t.StartsWith("!"))
                .Select(t => t.Substring(1).ToLower())
                .ToList();

            if (inclusionTerms.Any())
            {
                filteredItems = filteredItems
                    .Where(i => inclusionTerms.Any(term => 
                        i.Name.ToLower().Contains(term) || 
                        i.Category.Name.ToLower().Contains(term)))
                    .ToList();
            }

            if (exclusionTerms.Any())
            {
                filteredItems = filteredItems
                    .Where(i => !exclusionTerms.Any(term => 
                        i.Name.ToLower().Contains(term) || 
                        i.Category.Name.ToLower().Contains(term)))
                    .ToList();
            }
        }

        Items = filteredItems;
    }
}

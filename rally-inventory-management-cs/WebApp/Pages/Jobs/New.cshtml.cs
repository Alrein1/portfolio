using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace exam.Pages.Jobs;

public class NewJobModel : PageModel
{
    private readonly IJobRepository _jobRepository;
    private readonly IItemRepository _itemRepository;

    public NewJobModel(IJobRepository jobRepository, IItemRepository itemRepository)
    {
        _jobRepository = jobRepository;
        _itemRepository = itemRepository;
    }

    [BindProperty]
    public Job? NewJob { get; set; }
    public List<Item>? Items { get; set; }

    public void OnGet()
    {
        Items = _itemRepository.GetItems();
        NewJob = new Job
        {
            RequiredItems = new List<RequiredItem>()
        };
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrEmpty(NewJob!.Title))
        {
            ModelState.AddModelError("NewJob.Title", "Title is required");
            Items = _itemRepository.GetItems();
            return Page();
        }

        NewJob.RequiredItems = NewJob.RequiredItems!
            .Where(ri => ri.ItemId != 0 && ri.ItemQuantity > 0)
            .ToList();

        if (!NewJob.RequiredItems.Any())
        {
            ModelState.AddModelError("", "At least one item is required");
            Items = _itemRepository.GetItems();
            return Page();
        }

        foreach (var ri in NewJob.RequiredItems)
        {
            var item = _itemRepository.GetItemById(ri.ItemId);
            if (item == null)
            {
                ModelState.AddModelError("", $"Item with ID {ri.ItemId} not found");
                Items = _itemRepository.GetItems();
                return Page();
            }
            ri.Item = item;
        }

        var success = _jobRepository.AddJob(NewJob);
        
        if (!success)
        {
            ModelState.AddModelError("", "Failed to save the job. Please try again.");
            Items = _itemRepository.GetItems();
            return Page();
        }

        return RedirectToPage("./Index");
    }
}
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class JobHistoryModel : PageModel
{
    private readonly IJobRepository _jobRepository;

    public JobHistoryModel(IJobRepository jobRepository)
    {
        _jobRepository = jobRepository;
    }

    public List<JobHistory>? JobHistory { get; set; }
    public List<MaterialSummary>? MaterialsSummary { get; set; }
    public double TotalCost { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? FromDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? ToDate { get; set; }

    public void OnGet()
    {
        JobHistory = _jobRepository.GetJobHistory(FromDate, ToDate);
        TotalCost = JobHistory.Sum(j => j.TotalCost);

        MaterialsSummary = JobHistory
            .SelectMany(j => j.UsedItems)
            .GroupBy(i => new { i.ItemName, i.CategoryName })
            .Select(g => new MaterialSummary
            {
                ItemName = g.Key.ItemName,
                CategoryName = g.Key.CategoryName,
                TotalQuantity = g.Sum(i => i.Quantity),
                TotalCost = g.Sum(i => i.Quantity * i.PriceAtTime)
            })
            .OrderBy(m => m.CategoryName)
            .ThenBy(m => m.ItemName)
            .ToList();
    }
}

public class MaterialSummary
{
    public string? ItemName { get; set; }
    public string? CategoryName { get; set; }
    public int TotalQuantity { get; set; }
    public double TotalCost { get; set; }
}
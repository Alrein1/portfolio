using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace exam.Pages.Jobs;

public class JobsModel : PageModel
{
    private readonly IJobRepository _jobRepository;
    private readonly IItemRepository _itemRepository;

    public JobsModel(IJobRepository jobRepository, IItemRepository itemRepository)
    {
        _jobRepository = jobRepository;
        _itemRepository = itemRepository;
    }

    public List<Job>? Jobs { get; set; }
    [TempData]
    public string? StatusMessage { get; set; }

    public void OnGet()
    {
        Jobs = _jobRepository.GetJobs();
    }

    public IActionResult OnPostDelete(int jobId)
    {   
        var job = _jobRepository.GetJobById(jobId);
        if (job == null)
        {
            StatusMessage = "Error: Job not found.";
            return RedirectToPage();
        }
        var result = _jobRepository.DeleteJob(job);
        if (result)
        {
            StatusMessage = $"Job '{job.Title}' deleted successfully.";
        }
        return RedirectToPage();
    }
    public IActionResult OnPostPerform(int jobId)
    {
        var job = _jobRepository.GetJobById(jobId);
        if (job == null)
        {
            StatusMessage = "Error: Job not found.";
            return RedirectToPage();
        }

        var result = _jobRepository.PerformJob(jobId);
        if (result)
        {
            StatusMessage = $"Job '{job.Title}' performed successfully.";
        }

        return RedirectToPage();

    }
}
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class JobRepository : IJobRepository
{
    private readonly AppDbContext _context;
    private readonly IItemRepository _itemRepository;

    public JobRepository(AppDbContext context, IItemRepository itemRepository)
    {
        _context = context;
        _itemRepository = itemRepository;
    }
    public Job? GetJobById(int id)
    {
        return _context.PredefinedJobs
            .Include(j => j.RequiredItems)!
            .ThenInclude(ri => ri.Item)!
            .ThenInclude(i => i!.Category)!
            .Include(j => j.RequiredItems)!
            .ThenInclude(ri => ri.Item)
            .ThenInclude(i => i!.Location)
            .FirstOrDefault(j => j.Id == id);
    }
    public List<Job> GetJobs()
    {
        return _context.PredefinedJobs
            .Include(j => j.RequiredItems)!
            .ThenInclude(ri => ri.Item)
            .ThenInclude(i => i!.Category)
            .Include(j => j.RequiredItems)!
            .ThenInclude(ri => ri.Item)
            .ThenInclude(i => i!.Location)
            .ToList();
    }

    public bool AddJob(Job job)
    {
        try
        {
            _context.PredefinedJobs.Add(job);
            _context.SaveChanges();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
        return true;
    }
    public bool UpdateJob(Job job)
    {
        try
        {
            _context.PredefinedJobs.Update(job);
            _context.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
    public bool DeleteJob(Job job)
    {
        try
        {
            _context.PredefinedJobs.Remove(job);
            _context.SaveChanges();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
    public bool PerformJob(int jobId)
    {
        using var transaction = _context.Database.BeginTransaction();
        try
        {
            var job = _context.PredefinedJobs
                .Include(j => j.RequiredItems)!
                    .ThenInclude(ri => ri.Item)
                .FirstOrDefault(j => j.Id == jobId);

            if (job == null) return false;

            foreach (var requiredItem in job.RequiredItems!)
            {
                if (requiredItem.Item!.Quantity < requiredItem.ItemQuantity)
                {
                    return false;
                }
            }

            var jobHistory = new JobHistory
            {
                JobTitle = job.Title!,
                PerformedAt = DateTime.UtcNow,
                UsedItems = job.RequiredItems.Select(ri => new UsedItem
                {
                    ItemName = ri.Item!.Name,
                    Quantity = ri.ItemQuantity,
                    PriceAtTime = ri.Item.Price,
                    CategoryName = ri.Item.Category.Name
                }).ToList()
            };

            foreach (var requiredItem in job.RequiredItems)
            {
                requiredItem.Item!.Quantity -= requiredItem.ItemQuantity;
                _itemRepository.UpdateItem(requiredItem.Item);
            }
            
            _context.JobHistory.Add(jobHistory);
            job.TimesPerformed++;
            _context.PredefinedJobs.Update(job);
            _context.SaveChanges();
            transaction.Commit();
            
            return true;
        }
        catch (Exception)
        {
            transaction.Rollback();
            return false;
        }
    }

    public List<JobHistory> GetJobHistory(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.JobHistory
            .Include(jh => jh.UsedItems)
            .AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(jh => jh.PerformedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(jh => jh.PerformedAt <= toDate.Value);

        return query.OrderByDescending(jh => jh.PerformedAt).ToList();
    }
}
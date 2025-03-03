using Domain;

namespace DAL;

public interface IJobRepository
{
    List<Job> GetJobs();
    Job? GetJobById(int id);
    bool UpdateJob(Job job);
    bool AddJob(Job job);
    bool DeleteJob(Job job);
    bool PerformJob(int jobId);
    List<JobHistory> GetJobHistory(DateTime? fromDate = null, DateTime? toDate = null);
}
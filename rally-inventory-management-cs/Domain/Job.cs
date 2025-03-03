namespace Domain;

public class Job
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public List<RequiredItem>? RequiredItems { get; set; } // item:required amount
    public int TimesPerformed { get; set; } = 0;
    public double TotalPrice => RequiredItems?.Sum(ri => ri.Item?.Price * ri.ItemQuantity) ?? 0;
    
}
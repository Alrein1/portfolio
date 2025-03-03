namespace Domain;

public class JobHistory
{
    public int Id { get; set; }
    public string JobTitle { get; set; } = default!;
    public DateTime PerformedAt { get; set; }
    public List<UsedItem> UsedItems { get; set; } = new();
    public double TotalCost => UsedItems?.Sum(ui => ui.Quantity * ui.PriceAtTime) ?? 0;
}

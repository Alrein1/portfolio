namespace Domain;

public class UsedItem
{
    public int Id { get; set; }
    public string ItemName { get; set; } = default!;
    public int Quantity { get; set; }
    public double PriceAtTime { get; set; }
    public string CategoryName { get; set; } = default!;
}
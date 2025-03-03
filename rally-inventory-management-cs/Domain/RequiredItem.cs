namespace Domain;

public class RequiredItem
{
    public int Id { get; set; }
    public Item? Item { get; set; }
    public int ItemId { get; set; }
    public int ItemQuantity { get; set; }
    public override string ToString()
    {
        return $"{Item!.Name}/{ItemQuantity}";
    }
}
namespace Domain;

public class Item
{
    public int Id { get; set; }
    public String Name { get; set; } = default!;
    public int Quantity { get; set; } = 0;
    public int OptimalQuantity { get; set; } = 0;
    public double Price { get; set; } = default;
    public ItemCategory Category { get; set; } = default!;
    public ItemLocation Location { get; set; } = default!;
    public int CategoryId { get; set; }
    public int LocationId { get; set; }

    public override String ToString()
    {
        return $"{Name} (${Category}): {Quantity}/({OptimalQuantity}) in ${Location}";
    }
    
}
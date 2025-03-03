namespace Domain;

public class ItemCategory
{
   public int Id { get; set; }
   public string Name { get; set; } = default!;
   public override string ToString()
   {
      return Name;
   }
}
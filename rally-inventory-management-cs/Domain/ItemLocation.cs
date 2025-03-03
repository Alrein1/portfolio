namespace Domain;

public class ItemLocation
{
   public int Id { get; set; }
   public int Van { get; set; } = default!;
   public int Shelf { get; set; } = default!;
   public override string ToString()
   {
      return $"Van nr: {Van}, Shelf: {Shelf}";
   }
}
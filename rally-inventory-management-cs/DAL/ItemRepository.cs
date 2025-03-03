using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class ItemRepository : IItemRepository
    {
        private readonly AppDbContext _context;

        public ItemRepository(AppDbContext context)
        {
            _context = context;
        }
        
        public Item? GetItemById(int id)
        {
            return _context.Items!
                .Include(i => i.Category)!
                .Include(i => i.Location)!
                .FirstOrDefault(i => i.Id == id);
        }

        public List<Item> GetItems()
        {
            return _context.Items
                .Include(i => i.Category)
                .Include(i => i.Location)
                .ToList();
        }
        
        public bool AddItem(Item item)
        {
            try
            {
                _context.Items.Add(item);
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public bool UpdateItem(Item item)
        {
            try
            {
                _context.Items.Update(item);
                _context.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
    }
}
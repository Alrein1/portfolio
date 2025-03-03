using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppDbContext : DbContext
{
    public DbSet<Item> Items { get; set; } = default!;
    public DbSet<ItemCategory> ItemCategories { get; set; } = default!;
    public DbSet<Job> PredefinedJobs { get; set; } = default!;
    public DbSet<JobHistory> JobHistory { get; set; }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Job>()
            .Ignore(j => j.TotalPrice);
        modelBuilder.Entity<ItemCategory>().HasData(
        new ItemCategory { Id = 1, Name = "Car Part" },
        new ItemCategory { Id = 2, Name = "Fastener" },
        new ItemCategory { Id = 3, Name = "Fluid" }
    );

    // Seed mock data for ItemLocation
    modelBuilder.Entity<ItemLocation>().HasData(
        new ItemLocation { Id = 1, Van = 1, Shelf = 1 },
        new ItemLocation { Id = 2, Van = 1, Shelf = 2 },
        new ItemLocation { Id = 3, Van = 2, Shelf = 1 },
        new ItemLocation { Id = 4, Van = 2, Shelf = 2 }
    );

    // Seed mock data for Items with explicit Ids
    modelBuilder.Entity<Item>().HasData(
        new Item
        {
            Id = 1, // Explicit Id
            Name = "Gearbox",
            Quantity = 5,
            OptimalQuantity = 10,
            Price = 1500.00,
            CategoryId = 1, // Car Part
            LocationId = 1  // Van 1, Shelf 1
        },
        new Item
        {
            Id = 2, // Explicit Id
            Name = "Clutch Plate",
            Quantity = 8,
            OptimalQuantity = 15,
            Price = 200.00,
            CategoryId = 1, // Car Part
            LocationId = 2  // Van 1, Shelf 2
        },
        new Item
        {
            Id = 3, // Explicit Id
            Name = "Brake Pad",
            Quantity = 20,
            OptimalQuantity = 30,
            Price = 50.00,
            CategoryId = 1, // Car Part
            LocationId = 3  // Van 2, Shelf 1
        },
        new Item
        {
            Id = 4, // Explicit Id
            Name = "Engine Oil",
            Quantity = 25,
            OptimalQuantity = 40,
            Price = 15.00,
            CategoryId = 3, // Fluid
            LocationId = 4  // Van 2, Shelf 2
        },
        new Item
        {
            Id = 5, // Explicit Id
            Name = "Bolt M10x50",
            Quantity = 100,
            OptimalQuantity = 200,
            Price = 0.50,
            CategoryId = 2, // Fastener
            LocationId = 1  // Van 1, Shelf 1
        },
        new Item
        {
            Id = 6, // Explicit Id
            Name = "Nut M10",
            Quantity = 150,
            OptimalQuantity = 250,
            Price = 0.30,
            CategoryId = 2, // Fastener
            LocationId = 2  // Van 1, Shelf 2
        },
        new Item
        {
            Id = 7, // Explicit Id
            Name = "Oil Filter",
            Quantity = 10,
            OptimalQuantity = 20,
            Price = 20.00,
            CategoryId = 1, // Car Part
            LocationId = 3  // Van 2, Shelf 1
        },
        new Item
        {
            Id = 8, // Explicit Id
            Name = "Timing Belt",
            Quantity = 6,
            OptimalQuantity = 12,
            Price = 120.00,
            CategoryId = 1, // Car Part
            LocationId = 4  // Van 2, Shelf 2
        },
        new Item
        {
            Id = 9, // Explicit Id
            Name = "Radiator Hose",
            Quantity = 12,
            OptimalQuantity = 20,
            Price = 35.00,
            CategoryId = 1, // Car Part
            LocationId = 1  // Van 1, Shelf 1
        },
        new Item
        {
            Id = 10, // Explicit Id
            Name = "Spark Plug",
            Quantity = 40,
            OptimalQuantity = 60,
            Price = 10.00,
            CategoryId = 1, // Car Part
            LocationId = 2  // Van 1, Shelf 2
        }
    );
    }

    
    
}
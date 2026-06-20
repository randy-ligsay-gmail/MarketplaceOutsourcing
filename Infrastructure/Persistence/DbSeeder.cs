using MarketplaceOutsourcing.Domain.Entities;

namespace MarketplaceOutsourcing.Infrastructure.Persistence;

public static class DbSeeder
{
    public static void Seed(MarketplaceDbContext context)
    {
        if (context.Jobs.Any())
        {
            return;
        }

        var customerAlice = new Customer("John", "Dela");
        var customerBob = new Customer("Bobby", "Smith");

        var contractorJane = new Contractor("Adryn's Roofing Co.", 4.8m);
        var contractorMike = new Contractor("Ydnar Build Services", 4.5m);

        context.Customers.AddRange(customerAlice, customerBob);
        context.Contractors.AddRange(contractorJane, contractorMike);
        context.SaveChanges();

        var fixRoof = new Job(
            title: "Fix roof leak",
            description: "Repair shingles and seal flashing on the garage roof.",
            customerId: customerAlice.Id,
            startDate: DateTime.UtcNow.Date,
            dueDate: DateTime.UtcNow.Date.AddDays(14),
            budget: 1200m);

        var paintHouse = new Job(
            title: "Paint living room",
            description: "Repaint walls and trim in the living room.",
            customerId: customerAlice.Id,
            startDate: DateTime.UtcNow.Date.AddDays(3),
            dueDate: DateTime.UtcNow.Date.AddDays(10),
            budget: 800m);

        var buildDeck = new Job(
            title: "Build backyard deck",
            description: "Install a 12x10 wooden deck with railing.",
            customerId: customerBob.Id,
            startDate: DateTime.UtcNow.Date.AddDays(7),
            dueDate: DateTime.UtcNow.Date.AddDays(30),
            budget: 4500m);

        context.Jobs.AddRange(fixRoof, paintHouse, buildDeck);
        context.SaveChanges();

        context.JobOffers.AddRange(
            new JobOffer(fixRoof.Id, contractorJane.Id, 1150m),
            new JobOffer(fixRoof.Id, contractorMike.Id, 1300m),
            new JobOffer(buildDeck.Id, contractorMike.Id, 4200m));

        context.SaveChanges();

        Console.WriteLine("Seeded sample customers, contractors, jobs, and offers.");
    }
}

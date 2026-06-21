using MarketplaceOutsourcing.Domain.Entities;
using MarketplaceOutsourcing.Infrastructure.Auth;

namespace MarketplaceOutsourcing.Infrastructure.Persistence;

public static class DbSeeder
{
    private const string DefaultPassword = "Password123!";

    public static void Seed(MarketplaceDbContext context)
    {
        if (!context.Jobs.Any())
        {
            SeedSampleData(context);
        }

        SeedUsers(context);
    }

    private static void SeedSampleData(MarketplaceDbContext context)
    {
        var customerAlice = new Customer("John", "Dela");
        var customerBob = new Customer("Bobby", "Smith");
        var customerSarah = new Customer("Sarah", "Chen");
        var customerMarcus = new Customer("Marcus", "Rivera");

        var contractorJane = new Contractor("Adryn's Roofing Co.", 4.8m);
        var contractorMike = new Contractor("Ydnar Build Services", 4.5m);
        var contractorCloudNine = new Contractor("CloudNine DevOps LLC", 4.9m);
        var contractorSecureStack = new Contractor("SecureStack Cyber Solutions", 4.7m);
        var contractorPixelForge = new Contractor("PixelForge Web Studio", 4.6m);

        context.Customers.AddRange(customerAlice, customerBob, customerSarah, customerMarcus);
        context.Contractors.AddRange(
            contractorJane,
            contractorMike,
            contractorCloudNine,
            contractorSecureStack,
            contractorPixelForge);
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

        var migrateApi = new Job(
            title: "Migrate legacy API to .NET 9",
            description: "Refactor an existing ASP.NET Core 3.1 monolith to .NET 9 with clean architecture and OpenAPI docs.",
            customerId: customerSarah.Id,
            startDate: DateTime.UtcNow.Date.AddDays(5),
            dueDate: DateTime.UtcNow.Date.AddDays(45),
            budget: 12000m);

        var cicdPipeline = new Job(
            title: "Setup CI/CD pipeline on Azure DevOps",
            description: "Configure build, test, and deploy pipelines for a microservices backend with staging and production environments.",
            customerId: customerSarah.Id,
            startDate: DateTime.UtcNow.Date.AddDays(2),
            dueDate: DateTime.UtcNow.Date.AddDays(21),
            budget: 5500m);

        var pentest = new Job(
            title: "Penetration test for customer portal",
            description: "Perform OWASP Top 10 testing on a React + REST API portal, deliver findings report and remediation guidance.",
            customerId: customerMarcus.Id,
            startDate: DateTime.UtcNow.Date.AddDays(10),
            dueDate: DateTime.UtcNow.Date.AddDays(25),
            budget: 7500m);

        var reactDashboard = new Job(
            title: "Build internal analytics dashboard",
            description: "Create a React TypeScript dashboard with charts, role-based views, and integration to existing REST endpoints.",
            customerId: customerMarcus.Id,
            startDate: DateTime.UtcNow.Date.AddDays(7),
            dueDate: DateTime.UtcNow.Date.AddDays(35),
            budget: 9800m);

        var dbOptimization = new Job(
            title: "Optimize PostgreSQL database performance",
            description: "Analyze slow queries, add indexes, tune connection pooling, and document backup and recovery procedures.",
            customerId: customerBob.Id,
            startDate: DateTime.UtcNow.Date.AddDays(4),
            dueDate: DateTime.UtcNow.Date.AddDays(18),
            budget: 4200m);

        var ssoIntegration = new Job(
            title: "Implement SSO with JWT and OAuth2",
            description: "Integrate Azure AD OAuth2 login, issue JWT access tokens, and protect existing REST endpoints with RBAC.",
            customerId: customerMarcus.Id,
            startDate: DateTime.UtcNow.Date,
            dueDate: DateTime.UtcNow.Date.AddDays(28),
            budget: 6500m);

        context.Jobs.AddRange(
            fixRoof,
            paintHouse,
            buildDeck,
            migrateApi,
            cicdPipeline,
            pentest,
            reactDashboard,
            dbOptimization,
            ssoIntegration);
        context.SaveChanges();

        context.JobOffers.AddRange(
            new JobOffer(fixRoof.Id, contractorJane.Id, 1150m),
            new JobOffer(fixRoof.Id, contractorMike.Id, 1300m),
            new JobOffer(buildDeck.Id, contractorMike.Id, 4200m),
            new JobOffer(migrateApi.Id, contractorCloudNine.Id, 11500m),
            new JobOffer(migrateApi.Id, contractorPixelForge.Id, 11800m),
            new JobOffer(cicdPipeline.Id, contractorCloudNine.Id, 5200m),
            new JobOffer(cicdPipeline.Id, contractorSecureStack.Id, 5400m),
            new JobOffer(pentest.Id, contractorSecureStack.Id, 7200m),
            new JobOffer(reactDashboard.Id, contractorPixelForge.Id, 9500m),
            new JobOffer(reactDashboard.Id, contractorCloudNine.Id, 9100m),
            new JobOffer(dbOptimization.Id, contractorCloudNine.Id, 4000m),
            new JobOffer(ssoIntegration.Id, contractorSecureStack.Id, 6300m),
            new JobOffer(ssoIntegration.Id, contractorCloudNine.Id, 6100m));

        context.SaveChanges();

        Console.WriteLine("Seeded sample customers, contractors, jobs, and offers.");
    }

    private static void SeedUsers(MarketplaceDbContext context)
    {
        if (context.Users.Any())
        {
            return;
        }

        var hasher = new PasswordHasher();
        var passwordHash = hasher.Hash(DefaultPassword);

        var users = new List<User>
        {
            User.CreateAdmin("admin@example.com", passwordHash)
        };

        AddCustomerUser(users, context, passwordHash, "Dela", "john.dela@example.com");
        AddCustomerUser(users, context, passwordHash, "Smith", "bobby.smith@example.com");
        AddCustomerUser(users, context, passwordHash, "Chen", "sarah.chen@example.com");
        AddCustomerUser(users, context, passwordHash, "Rivera", "marcus.rivera@example.com");

        AddContractorUser(users, context, passwordHash, "Roofing", "jane@example.com");
        AddContractorUser(users, context, passwordHash, "Build Services", "mike@example.com");
        AddContractorUser(users, context, passwordHash, "CloudNine DevOps", "cloudnine@example.com");
        AddContractorUser(users, context, passwordHash, "SecureStack Cyber", "securestack@example.com");
        AddContractorUser(users, context, passwordHash, "PixelForge Web", "pixelforge@example.com");

        context.Users.AddRange(users);
        context.SaveChanges();

        Console.WriteLine($"Seeded {users.Count} auth users. Default password: {DefaultPassword}");
    }

    private static void AddCustomerUser(
        List<User> users,
        MarketplaceDbContext context,
        string passwordHash,
        string lastName,
        string email)
    {
        var customer = context.Customers.FirstOrDefault(c => c.LastName == lastName);
        if (customer is not null)
        {
            users.Add(User.CreateCustomer(email, passwordHash, customer.Id));
        }
    }

    private static void AddContractorUser(
        List<User> users,
        MarketplaceDbContext context,
        string passwordHash,
        string nameFragment,
        string email)
    {
        var contractor = context.Contractors.FirstOrDefault(c => c.Name.Contains(nameFragment));
        if (contractor is not null)
        {
            users.Add(User.CreateContractor(email, passwordHash, contractor.Id));
        }
    }
}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ExpenseAI.Domain.Entities;
using ExpenseAI.Domain.ValueObjects;
using CategoryEntity = ExpenseAI.Domain.Entities.Category;

namespace ExpenseAI.Infrastructure.Data;

/// <summary>
/// Application database context
/// </summary>
public class ApplicationDbContext : IdentityDbContext<ExpenseAIIdentityUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // DbSets for domain entities
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Budget> Budgets { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure ExpenseAIIdentityUser
        builder.Entity<ExpenseAIIdentityUser>(entity =>
        {
            entity.ToTable("AspNetUsers");
            entity.Property(e => e.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.LastName).HasMaxLength(50).IsRequired();
            entity.Property(e => e.TimeZone).HasMaxLength(50).HasDefaultValue("UTC");
            entity.Property(e => e.PreferredCurrency).HasMaxLength(3).HasDefaultValue("USD");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            // Indexes
            entity.HasIndex(e => e.Email).IsUnique();
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.IsActive);
        });

        // Configure Expense entity
        builder.Entity<Expense>(entity =>
        {
            entity.ToTable("Expenses");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Description).HasMaxLength(500).IsRequired();
            entity.Property(e => e.Notes).HasMaxLength(2000);
            entity.Property(e => e.ReceiptUrl).HasMaxLength(1000);
            entity.Property(e => e.MerchantName).HasMaxLength(200);
            entity.Property(e => e.PaymentMethod).HasMaxLength(50);
            entity.Property(e => e.RejectionReason).HasMaxLength(1000);
            entity.Property(e => e.AiExtractedData).HasColumnType("nvarchar(max)");
            entity.Property(e => e.UserId).IsRequired();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            // Configure Money value object
            entity.OwnsOne(e => e.Amount, money =>
            {
                money.Property(m => m.Amount).HasColumnType("decimal(18,2)").IsRequired();
                money.Property(m => m.Currency).HasMaxLength(3).IsRequired();
            });

            // Configure Category value object (owned)
            entity.OwnsOne(e => e.Category, category =>
            {
                category.Property(c => c.Name).HasColumnName("CategoryName").HasMaxLength(100);
                category.Property(c => c.Description).HasColumnName("CategoryDescription").HasMaxLength(500);
                category.Property(c => c.Color).HasColumnName("CategoryColor").HasMaxLength(7);
                category.Property(c => c.Icon).HasColumnName("CategoryIcon").HasMaxLength(50);
                category.Property(c => c.IsSystemCategory).HasColumnName("IsSystemCategory");
            });

            // Configure Tags as JSON
            entity.Property(e => e.Tags)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                .HasMaxLength(1000);

            // Relationships
            entity.HasOne(e => e.User)
                .WithMany(u => u.Expenses)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.ExpenseDate);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.UserId, e.ExpenseDate });
        });

        // Configure Category entity
        builder.Entity<CategoryEntity>(entity =>
        {
            entity.ToTable("Categories");
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Id).ValueGeneratedOnAdd();
            entity.Property(c => c.Name).HasMaxLength(100).IsRequired();
            entity.Property(c => c.Description).HasMaxLength(500);
            entity.Property(c => c.Color).HasMaxLength(7); // Hex color code
            entity.Property(c => c.Icon).HasMaxLength(50);
            entity.Property(c => c.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(c => c.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            // Indexes
            entity.HasIndex(c => c.Name).IsUnique();
            entity.HasIndex(c => c.IsSystemCategory);
        });

        // Configure Invoice entity
        builder.Entity<Invoice>(entity =>
        {
            entity.ToTable("Invoices");
            entity.HasKey(i => i.Id);

            entity.Property(i => i.Id).ValueGeneratedOnAdd();
            entity.Property(i => i.InvoiceNumber).HasMaxLength(50).IsRequired();
            entity.Property(i => i.ClientName).HasMaxLength(200).IsRequired();
            entity.Property(i => i.ClientEmail).HasMaxLength(254).IsRequired();
            entity.Property(i => i.ClientAddress).HasMaxLength(1000);
            entity.Property(i => i.Notes).HasMaxLength(2000);
            entity.Property(i => i.PaymentMethod).HasMaxLength(50);
            entity.Property(i => i.PaymentReference).HasMaxLength(100);
            entity.Property(i => i.UserId).IsRequired();
            entity.Property(i => i.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(i => i.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            // Configure Money value objects
            entity.OwnsOne(i => i.SubtotalAmount, money =>
            {
                money.Property(m => m.Amount).HasColumnName("SubtotalAmount").HasColumnType("decimal(18,2)").IsRequired();
                money.Property(m => m.Currency).HasColumnName("SubtotalCurrency").HasMaxLength(3).IsRequired();
            });

            entity.OwnsOne(i => i.TaxAmount, money =>
            {
                money.Property(m => m.Amount).HasColumnName("TaxAmount").HasColumnType("decimal(18,2)").IsRequired();
                money.Property(m => m.Currency).HasColumnName("TaxCurrency").HasMaxLength(3).IsRequired();
            });

            entity.OwnsOne(i => i.TotalAmount, money =>
            {
                money.Property(m => m.Amount).HasColumnName("TotalAmount").HasColumnType("decimal(18,2)").IsRequired();
                money.Property(m => m.Currency).HasColumnName("TotalCurrency").HasMaxLength(3).IsRequired();
            });

            // Relationships
            // entity.HasOne(i => i.User)
            //     .WithMany(u => u.Invoices)
            //     .HasForeignKey(i => i.UserId)
            //     .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(i => i.UserId);
            entity.HasIndex(i => i.InvoiceNumber).IsUnique();
            entity.HasIndex(i => i.IssueDate);
            entity.HasIndex(i => i.DueDate);
            entity.HasIndex(i => i.Status);
        });

        // Configure Budget entity
        builder.Entity<Budget>(entity =>
        {
            entity.ToTable("Budgets");
            entity.HasKey(b => b.Id);

            entity.Property(b => b.Id).ValueGeneratedOnAdd();
            entity.Property(b => b.Name).HasMaxLength(100).IsRequired();
            entity.Property(b => b.Description).HasMaxLength(500);
            entity.Property(b => b.Category).HasMaxLength(100).IsRequired();
            entity.Property(b => b.AlertThreshold).HasColumnType("decimal(5,2)");
            entity.Property(b => b.UserId).IsRequired();
            entity.Property(b => b.Color).HasMaxLength(7);
            entity.Property(b => b.Icon).HasMaxLength(50);
            entity.Property(b => b.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            entity.Property(b => b.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");

            // Configure Money value object
            entity.OwnsOne(b => b.Amount, money =>
            {
                money.Property(m => m.Amount).HasColumnType("decimal(18,2)").IsRequired();
                money.Property(m => m.Currency).HasMaxLength(3).IsRequired();
            });

            // Configure Tags as JSON
            entity.Property(b => b.Tags)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                .HasMaxLength(1000);

            // Relationships
            entity.HasOne(b => b.User)
                .WithMany(u => u.Budgets)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            entity.HasIndex(b => b.UserId);
            entity.HasIndex(b => b.Category);
            entity.HasIndex(b => b.StartDate);
            entity.HasIndex(b => b.EndDate);
            entity.HasIndex(b => b.IsActive);
        });

        // Seed default categories
        SeedData(builder);
    }

    private static void SeedData(ModelBuilder builder)
    {
        var defaultCategories = CategoryEntity.GetSystemCategories()
            .Select((cat, index) => new
            {
                Id = Guid.NewGuid(),
                cat.Name,
                cat.Description,
                cat.Color,
                cat.Icon,
                cat.IsSystemCategory,
                cat.IsActive,
                cat.SortOrder,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToArray();

        builder.Entity<CategoryEntity>().HasData(defaultCategories);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Update timestamps
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            if (entry.Entity is ExpenseAIIdentityUser user)
            {
                if (entry.State == EntityState.Added)
                    user.CreatedAt = DateTime.UtcNow;
                user.UpdatedAt = DateTime.UtcNow;
            }

            // TODO: Fix timestamp updates for BaseEntity classes
            // if (entry.Entity is Expense expense)
            // {
            //     if (entry.State == EntityState.Added)
            //         expense.CreatedAt = DateTime.UtcNow;
            //     expense.UpdatedAt = DateTime.UtcNow;
            // }

            // if (entry.Entity is Budget budget)
            // {
            //     if (entry.State == EntityState.Added)
            //         budget.CreatedAt = DateTime.UtcNow;
            //     budget.UpdatedAt = DateTime.UtcNow;
            // }

            // if (entry.Entity is Invoice invoice)
            // {
            //     if (entry.State == EntityState.Added)
            //         invoice.CreatedAt = DateTime.UtcNow;
            //     invoice.UpdatedAt = DateTime.UtcNow;
            // }

            if (entry.Entity is CategoryEntity category)
            {
                // TODO: Fix timestamp updates for BaseEntity classes
                // if (entry.State == EntityState.Added)
                //     category.CreatedAt = DateTime.UtcNow;
                // category.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}

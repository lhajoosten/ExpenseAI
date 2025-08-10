using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ExpenseAI.Domain.Entities;

namespace ExpenseAI.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(254);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.ProfileImageUrl)
            .HasMaxLength(500);

        builder.Property(u => u.PreferredCurrency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        builder.Property(u => u.TimeZone)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("UTC");

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.UpdatedAt);

        builder.Property(u => u.LastLoginAt);

        builder.Property(u => u.CreatedBy)
            .HasMaxLength(100);

        builder.Property(u => u.UpdatedBy)
            .HasMaxLength(100);

        // Navigation properties
        builder.HasMany(u => u.Expenses)
            .WithOne(e => e.User)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Custom categories (owned entities)
        builder.OwnsMany(u => u.CustomCategories, category =>
        {
            category.WithOwner();
            category.Property(c => c.Name).HasMaxLength(100);
            category.Property(c => c.Description).HasMaxLength(500);
            category.Property(c => c.Color).HasMaxLength(7);
            category.Property(c => c.Icon).HasMaxLength(50);
        });

        // Ignore domain events
        builder.Ignore(u => u.DomainEvents);
    }
}

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("Expenses");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(e => e.ExpenseDate)
            .IsRequired();

        builder.Property(e => e.Notes)
            .HasMaxLength(1000);

        builder.Property(e => e.ReceiptUrl)
            .HasMaxLength(500);

        builder.Property(e => e.MerchantName)
            .HasMaxLength(200);

        builder.Property(e => e.PaymentMethod)
            .HasMaxLength(50);

        builder.Property(e => e.IsReimbursable)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.IsReimbursed)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.RejectionReason)
            .HasMaxLength(500);

        builder.Property(e => e.IsAiCategorized)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(e => e.AiConfidenceScore)
            .HasColumnType("float");

        builder.Property(e => e.AiExtractedData)
            .HasColumnType("nvarchar(max)");

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt);

        builder.Property(e => e.CreatedBy)
            .HasMaxLength(100);

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(100);

        // Tags as JSON column
        builder.Property(e => e.Tags)
            .HasConversion(
                tags => string.Join(';', tags),
                tags => tags.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList())
            .HasColumnName("Tags")
            .HasMaxLength(1000);

        // Ignore domain events
        builder.Ignore(e => e.DomainEvents);
    }
}

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.ClientName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.ClientEmail)
            .IsRequired()
            .HasMaxLength(254);

        builder.Property(i => i.ClientAddress)
            .HasMaxLength(500);

        builder.Property(i => i.IssueDate)
            .IsRequired();

        builder.Property(i => i.DueDate)
            .IsRequired();

        builder.Property(i => i.TaxRate)
            .IsRequired()
            .HasColumnType("float");

        builder.Property(i => i.Notes)
            .HasMaxLength(1000);

        builder.Property(i => i.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(i => i.PaidDate);

        builder.Property(i => i.PaymentMethod)
            .HasMaxLength(100);

        builder.Property(i => i.PaymentReference)
            .HasMaxLength(200);

        builder.Property(i => i.UserId)
            .IsRequired();

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.UpdatedAt);

        builder.Property(i => i.CreatedBy)
            .HasMaxLength(100);

        builder.Property(i => i.UpdatedBy)
            .HasMaxLength(100);

        // Navigation properties
        builder.HasMany(i => i.LineItems)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.User)
            .WithMany()
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events
        builder.Ignore(i => i.DomainEvents);
    }
}

public class InvoiceLineItemConfiguration : IEntityTypeConfiguration<InvoiceLineItem>
{
    public void Configure(EntityTypeBuilder<InvoiceLineItem> builder)
    {
        builder.ToTable("InvoiceLineItems");

        // Configure shadow property for the primary key
        builder.Property<Guid>("Id")
            .ValueGeneratedOnAdd();

        builder.HasKey("Id");

        // Configure foreign key to Invoice
        builder.Property<Guid>("InvoiceId")
            .IsRequired();

        builder.Property(li => li.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(li => li.Quantity)
            .IsRequired();

        // Configure UnitPrice as owned entity (Money value object)
        builder.OwnsOne(li => li.UnitPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("UnitPriceAmount")
                .HasColumnType("decimal(18,2)");

            money.Property(m => m.Currency)
                .HasColumnName("UnitPriceCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Configure TotalPrice as owned entity (Money value object)
        builder.OwnsOne(li => li.TotalPrice, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalPriceAmount")
                .HasColumnType("decimal(18,2)");

            money.Property(m => m.Currency)
                .HasColumnName("TotalPriceCurrency")
                .HasMaxLength(3)
                .IsRequired();
        });
    }
}

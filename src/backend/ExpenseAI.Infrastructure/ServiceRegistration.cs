using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ExpenseAI.Application.Interfaces;
using ExpenseAI.Infrastructure.Data;
using ExpenseAI.Infrastructure.Repositories;
using ExpenseAI.Infrastructure.AI;
using ExpenseAI.Infrastructure.Storage;
using ExpenseAI.Infrastructure.External;

namespace ExpenseAI.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<IExpenseRepository, ExpenseRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // AI Services
        services.Configure<OpenAiSettings>(configuration.GetSection("AI:OpenAI"));
        services.Configure<AzureCognitiveSettings>(configuration.GetSection("AI:Azure"));

        services.AddHttpClient<IAiExpenseCategorizationService, OpenAiExpenseCategorizationService>();
        services.AddHttpClient<IAiDocumentProcessingService, AzureDocumentProcessingService>();

        // External Services
        services.Configure<BlobStorageSettings>(configuration.GetSection("Storage:Azure"));
        services.Configure<EmailSettings>(configuration.GetSection("Email"));

        services.AddScoped<IFileStorageService, AzureBlobStorageService>();
        services.AddScoped<IEmailService, EmailService>();

        // Caching
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        return services;
    }
}

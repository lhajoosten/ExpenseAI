using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ExpenseAI.Application.Behaviors;
using ExpenseAI.Application.Validators.Auth;
using ExpenseAI.Application.Commands.Expenses.CreateExpense;
using ExpenseAI.Application.Commands.Expenses.UpdateExpense;
using ExpenseAI.Application.Commands.Expenses.DeleteExpense;
using ExpenseAI.Application.Commands.Expenses.UploadReceipt;

namespace ExpenseAI.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Add MediatR
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

        // Add FluentValidation validators - Expenses
        services.AddScoped<IValidator<CreateExpenseCommand>, CreateExpenseValidator>();
        services.AddScoped<IValidator<UpdateExpenseCommand>, UpdateExpenseValidator>();
        services.AddScoped<IValidator<DeleteExpenseCommand>, DeleteExpenseValidator>();
        services.AddScoped<IValidator<UploadReceiptCommand>, UploadReceiptValidator>();

        // Add FluentValidation validators - Auth
        services.AddScoped<IValidator<DTOs.Auth.LoginRequestDto>, LoginRequestDtoValidator>();
        services.AddScoped<IValidator<DTOs.Auth.RegisterRequestDto>, RegisterRequestDtoValidator>();

        // Add pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}

using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ExpenseAI.Application.Behaviors;
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

        // Add FluentValidation validators automatically
        services.AddValidatorsFromAssembly(assembly);

        // Add pipeline behaviors
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}

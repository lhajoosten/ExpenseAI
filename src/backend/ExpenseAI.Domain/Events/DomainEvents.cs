using MediatR;

namespace ExpenseAI.Domain.Events;

public record UserCreatedEvent(Guid UserId, string Email) : INotification;

public record UserDeactivatedEvent(Guid UserId, string Email) : INotification;

public record ExpenseCreatedEvent(Guid ExpenseId, Guid UserId, decimal Amount, string Currency) : INotification;

public record ExpenseUpdatedEvent(Guid ExpenseId, Guid UserId, decimal OldAmount, decimal NewAmount, string Currency) : INotification;

public record ExpenseDeletedEvent(Guid ExpenseId, Guid UserId) : INotification;

public record InvoiceGeneratedEvent(Guid InvoiceId, Guid UserId, decimal TotalAmount, string Currency) : INotification;

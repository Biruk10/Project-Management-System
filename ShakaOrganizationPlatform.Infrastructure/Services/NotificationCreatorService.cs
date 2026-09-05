using ShakaOrganizationPlatform.Application.Common.Interfaces;
using ShakaOrganizationPlatform.Domain.Entities;
using ShakaOrganizationPlatform.Infrastructure.Persistence;

namespace ShakaOrganizationPlatform.Infrastructure.Services;

public class NotificationCreatorService : INotificationCreator
{
    private readonly AppDbContext _context;

    public NotificationCreatorService(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(int organizationId, int userId, string title, string message, string? type = null, string? link = null, CancellationToken cancellationToken = default)
    {
        var notification = new Notification
        {
            OrganizationId = organizationId,
            UserId = userId,
            Title = title,
            Message = message,
            Type = type,
            Link = link,
            IsRead = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);
    }
}

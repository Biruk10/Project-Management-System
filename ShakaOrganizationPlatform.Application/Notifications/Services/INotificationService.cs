using ShakaOrganizationPlatform.Application.Common.Models;
using ShakaOrganizationPlatform.Application.Notifications.DTOs;

namespace ShakaOrganizationPlatform.Application.Notifications.Services;

public interface INotificationService
{
    Task<PagedResult<NotificationDto>> GetMyNotificationsAsync(NotificationFilterParams filters, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(int id, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

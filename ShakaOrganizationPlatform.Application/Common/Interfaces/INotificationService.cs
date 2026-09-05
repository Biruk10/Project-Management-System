namespace ShakaOrganizationPlatform.Application.Common.Interfaces;

public interface INotificationCreator
{
    Task CreateAsync(int organizationId, int userId, string title, string message, string? type = null, string? link = null, CancellationToken cancellationToken = default);
}

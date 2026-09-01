namespace ShakaOrganizationPlatform.Application.Common.Interfaces;

public interface ITenantService
{
    int? OrganizationId { get; }
    bool HasTenant { get; }
    void SetTenant(int organizationId);
}


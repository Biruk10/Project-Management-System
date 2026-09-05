using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ShakaOrganizationPlatform.Application.Common.Interfaces;

namespace ShakaOrganizationPlatform.Infrastructure.Services;

public class TenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private int? _explicitTenantId;

    public TenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? OrganizationId
    {
        get
        {
            if (_explicitTenantId.HasValue)
                return _explicitTenantId.Value;

            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null) return null;

            if (IsSystemAdmin(user)) return null;

            var orgClaim = user.FindFirst("OrganizationId")?.Value
                ?? user.FindFirst("organization_id")?.Value
                ?? user.FindFirst("tenant_id")?.Value;

            return int.TryParse(orgClaim, out var orgId) ? orgId : null;
        }
    }

    public bool HasTenant => OrganizationId.HasValue;

    public void SetTenant(int organizationId)
    {
        _explicitTenantId = organizationId;
    }

    private static bool IsSystemAdmin(ClaimsPrincipal user)
    {
        return user.FindFirst("IsSystemAdmin")?.Value == "true"
            || user.IsInRole("SystemAdmin");
    }
}

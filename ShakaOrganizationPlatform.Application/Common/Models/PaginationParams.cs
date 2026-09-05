namespace ShakaOrganizationPlatform.Application.Common.Models;

public class PaginationParams
{
    private int _pageSize = 20;
    private const int MaxPageSize = 100;

    public int Page { get; set; } = 1;

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    public string? Search { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}

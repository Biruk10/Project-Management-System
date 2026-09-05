using ShakaOrganizationPlatform.Application.Common.Models;
using ShakaOrganizationPlatform.Application.Tasks.DTOs;

namespace ShakaOrganizationPlatform.Application.Tasks.Services;

public interface ITaskService
{
    Task<PagedResult<TaskDto>> GetAllAsync(TaskFilterParams filters, CancellationToken cancellationToken = default);
    Task<TaskDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TaskDto> CreateAsync(CreateTaskDto dto, CancellationToken cancellationToken = default);
    Task<TaskDto> UpdateAsync(int id, UpdateTaskDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

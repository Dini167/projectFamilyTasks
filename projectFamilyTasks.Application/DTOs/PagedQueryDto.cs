using System.ComponentModel.DataAnnotations;

namespace projectFamilyTasks.Application.DTOs;

public class PagedQueryDto
{
    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, 50)]
    public int PageSize { get; set; } = 10;

    public string? SortBy { get; set; }
    public bool Descending { get; set; }
}

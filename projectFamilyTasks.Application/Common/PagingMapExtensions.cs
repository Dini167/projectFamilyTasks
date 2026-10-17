using AutoMapper;
using projectFamilyTasks.Application.DTOs;

namespace projectFamilyTasks.Application.Common;

public static class PagingMapExtensions
{
    public static PagedResultDto<TDest> MapPaged<TSource, TDest>(
        this IMapper mapper, PagedResultDto<TSource> page)
    {
        return new PagedResultDto<TDest>
        {
            Items = mapper.Map<List<TDest>>(page.Items),
            TotalCount = page.TotalCount,
            Page = page.Page,
            PageSize = page.PageSize
        };
    }
}
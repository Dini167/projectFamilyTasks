using System.ComponentModel.DataAnnotations;
using projectFamilyTasks.Application.DTOs;

namespace projectFamilyTasks.Tests.Application;

public class PagedQueryDtoTests
{
    [Theory]
    [InlineData(1, 10, true)]
    [InlineData(100_000, 50, true)]
    [InlineData(0, 10, false)]
    [InlineData(int.MaxValue, 50, false)]
    [InlineData(1, 51, false)]
    public void Validation_PageAndPageSizeLimits(int page, int pageSize, bool expectedValid)
    {
        var query = new PagedQueryDto { Page = page, PageSize = pageSize };

        var isValid = Validator.TryValidateObject(query, new ValidationContext(query), null, true);

        Assert.Equal(expectedValid, isValid);
    }
}

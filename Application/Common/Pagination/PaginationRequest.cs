using System.ComponentModel.DataAnnotations;

namespace Application.Common.Pagination
{
    public sealed record PaginationRequest([Range(1,int.MaxValue)]int PageNumber = 1,
        [Range(1,50)]int PageSize = 10);
}

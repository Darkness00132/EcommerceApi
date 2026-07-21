namespace Application.Common.Pagination
{
    public record PaginationResult<T>(
        IReadOnlyList<T> Items,
        int PageNumber,
        int PageSize,
        int TotalCount)
    {
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    }
}
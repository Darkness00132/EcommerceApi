namespace Api.Contracts.Common;

public sealed class PaginationRequest
{
    private const int DefaultPageNumber = 1;
    private const int DefaultPageSize = 20;
    private const int MaxPageSize = 100;

    private int _pageNumber = DefaultPageNumber;
    private int _pageSize = DefaultPageSize;

    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = value < 1 ? DefaultPageNumber : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < 1
            ? DefaultPageSize
            : value > MaxPageSize
                ? MaxPageSize
                : value;
    }

    public int Skip => (PageNumber - 1) * PageSize;
}

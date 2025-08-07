using System;
using System.Collections.Generic;
using System.Linq;

namespace Accounting.Application.Common.Models
{
    public class PagedResult<T>
    {
        public PagedResult(IEnumerable<T> items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items?.ToList() ?? new List<T>();
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            HasPreviousPage = pageNumber > 1;
            HasNextPage = pageNumber < TotalPages;
        }

        public IReadOnlyList<T> Items { get; }
        public int TotalCount { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalPages { get; }
        public bool HasPreviousPage { get; }
        public bool HasNextPage { get; }
        public int ItemsCount => Items.Count;

        public static PagedResult<T> Empty(int pageNumber = 1, int pageSize = 10)
        {
            return new PagedResult<T>(Enumerable.Empty<T>(), 0, pageNumber, pageSize);
        }
    }

    public class PaginationRequest
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; } = string.Empty;
        public string SortBy { get; set; } = string.Empty;
        public bool SortDescending { get; set; } = false;

        public int Skip => (PageNumber - 1) * PageSize;
        public int Take => PageSize;

        public void Validate()
        {
            if (PageNumber < 1)
                PageNumber = 1;

            if (PageSize < 1)
                PageSize = 10;

            if (PageSize > 100)
                PageSize = 100;
        }
    }
}
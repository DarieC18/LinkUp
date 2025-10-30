﻿namespace LinkUp.Application.Common
{
    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int Total { get; init; }
        public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);
    }
}

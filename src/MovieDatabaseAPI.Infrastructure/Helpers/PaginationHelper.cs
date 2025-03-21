﻿using Microsoft.EntityFrameworkCore;
using MovieDatabaseAPI.Core.Helpers;

namespace MovieDatabaseAPI.Infrastructure.Helpers;

public static class PaginationHelper
{
    public static PagedList<T> Create<T>(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = source.Count();
        var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }

    public static async Task<PagedList<T>> CreateAsync<T>(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items = await source.Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedList<T>(items, count, pageNumber, pageSize);
    }
}
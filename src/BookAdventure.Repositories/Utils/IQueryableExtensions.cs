using System;
using BookAdventure.Dto.Request;

namespace BookAdventure.Repositories.Utils;

public static class IQueryableExtensions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> queryable, PaginationDto paginationDTO)
    {
        return queryable
            .Skip((paginationDTO.Page - 1) * paginationDTO.RecordsPerPage)
            .Take(paginationDTO.RecordsPerPage);
    }
}

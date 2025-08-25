using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace BookAdventure.Repositories.Utils;

public static class HttpContextExtensions
{
    public async static Task AddPaginationHeader<T>(this HttpContext httpContext, IQueryable<T> queryable)
    {
        if (httpContext == null)
            throw new ArgumentNullException(nameof(httpContext));

        List<T> totalRecords = await queryable.ToListAsync();
        httpContext.Response.Headers.Add("TotalRecordsQuantity", totalRecords.Count.ToString());//ojo con el nombre del parámetro que usaremos, más adelante lo usaremos en CORS
    }
}

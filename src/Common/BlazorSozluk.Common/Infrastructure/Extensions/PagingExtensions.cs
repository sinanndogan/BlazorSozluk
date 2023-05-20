using BlazorSozluk.Common.Models.Page;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BlazorSozluk.Common.Infrastructure.Extensions
{
    public static class PagingExtensions
    {
        public static async Task<PagedViewModel<T>> GetPaged <T>(this IQueryable<T> query,
                                                                 int currentPage,
                                                                 int pageSize) where T : class
        {
            //count totalpage için aldıgım count totalrowcount 
            var count = await query.CountAsync();

            Page paging = new(currentPage, pageSize, count);

             var data = await query.Skip(paging.Skip).Take(paging.PageSize).AsNoTracking().ToListAsync();

            var result = new PagedViewModel<T>(data,paging);

            return result;
        }
    }
}

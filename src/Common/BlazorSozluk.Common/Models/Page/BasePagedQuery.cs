using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Common.Models.Page
{
    public class BasePagedQuery
    {
        public BasePagedQuery(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }


        //kaçıncı sayfa
        public int Page { get; set; }
        //kaç eleman istiyoruz
        public int PageSize { get; set; }
    }
}

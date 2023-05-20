using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Common.Models.Page
{
    public class Page

    {
        public Page():this(0)
        {
            //Bu ctor hiç veri göndermediğmiz durumlarda çalışacaktır
        }

        public Page(int totalRowCount):this(1,10,totalRowCount)
        {
            
        }

        public Page(int pageSize, int totalRowCount):this(1,pageSize,totalRowCount)
        {

        }
        public Page(int currentPage, int pageSize, int totalRowCount)
        {
            if (currentPage < 1)
                throw new ArgumentException("Invalid page number!");

            if (pageSize < 1)
                throw new ArgumentException("Invalid page size!");

            this.TotalRowCount = totalRowCount;
            this.CurrentPage = currentPage;
            this.PageSize = pageSize;
        }

       

        //Bulundugumuz sayfa
        public  int  CurrentPage { get; set; }

        //Bir sayfada kaç tane eleman gösteriyor onu belirtiyor.
        public  int  PageSize { get; set; }

        //Toplam 100 adet kayıt var biz size suan 10' adet gösteriyoruz gibi düşenbilirsin.
        public  int  TotalRowCount { get; set; }

        //toplamda kaç adet sayfa olacağını burdan belirliyoruz 
        public int TotalPageCount => (int)Math.Ceiling((double)TotalRowCount / PageSize);

        //Şuan hangi sayfadaysak onun 1 eksiğine göre pagesize çarpımını getir   yani 50.sayfadaysan ilk 40 veriyi atla 
        public int Skip => (CurrentPage - 1) * PageSize;
    }
}

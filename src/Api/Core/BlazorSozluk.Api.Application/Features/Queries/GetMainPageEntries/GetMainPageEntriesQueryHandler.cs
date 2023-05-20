using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Common.Infrastructure.Extensions;
using BlazorSozluk.Common.Models.Page;
using BlazorSozluk.Common.Models.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BlazorSozluk.Api.Application.Features.Queries.GetMainPageEntries
{
    //pagedViewModel yaparak sayfalanmış bir şekilde dönmeyi sağladık
    public class GetMainPageEntriesQueryHandler : IRequestHandler<GetMainPageEntriesQuery, PagedViewModel<GetEntryDetailViewModel>>
    {
        private readonly IEntryRepository entryRepository;
       

        public GetMainPageEntriesQueryHandler(IEntryRepository entryRepository)
        {
            this.entryRepository = entryRepository;
            
        }
        public async Task<PagedViewModel<GetEntryDetailViewModel>> Handle(GetMainPageEntriesQuery request, CancellationToken cancellationToken)
        {
            //Burada Sorgumuzu getiriyoruz
            var query = entryRepository.AsQueryable();

            query = query.Include(i => i.EntryFavorites)
                       .Include(i => i.CreatedBy)
                       .Include(i => i.EntryVotes);

            var list = query.Select(i => new GetEntryDetailViewModel()
            {
                Id = i.Id,
                Subject=i.Subject,
                Content = i.Content,
                IsFavorited=request.UserId.HasValue && i.EntryFavorites.Any(a=>a.CreatedById==request.UserId),
                FavoritedCount=i.EntryFavorites.Count,
                CreatedDate=i.CreateDate,
                CreatedByUserName=i.CreatedBy.UserName,
                VoteType=
                request.UserId.HasValue && i.EntryVotes.Any(a=>a.CreatedById ==request.UserId)
                ? i.EntryVotes.FirstOrDefault(a=>a.CreatedById==request.UserId).VoteType
                :Common.ViewModels.VoteType.None,

            });

            var entries = await list.GetPaged(request.Page, request.PageSize);

            return new PagedViewModel<GetEntryDetailViewModel>(entries.Results, entries.PageInfo);
           

        }
    }
}

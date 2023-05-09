using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Api.Application.Features.Commands.EntryComment.CreateFav
{
    public class CreateEntryCommentFavCommand:IRequest<bool>
    {
        //hangi entry olduguunu belli eden entry ıd si
        public Guid EntryCommentId { get; set; }


        //hangi kullanıcı fav a ekliyor
        public Guid UserId { get; set; }

        public CreateEntryCommentFavCommand(Guid entryCommentId, Guid userId)
        {
            EntryCommentId = entryCommentId;
            UserId = userId;
        }
    }
}

using BlazorSozluk.Api.Application.Features.Queries.GetEntries;
using BlazorSozluk.Common.Models.RequestModels;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlazorSozluk.Api.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntryController : BaseController
    {

        private readonly IMediator mediator;

        public EntryController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetEntries([FromQuery] GetEntriesQuery query)
        {
            var data=await mediator.Send(query);

            return Ok(data);
        }





        [HttpPost]
        [Route("CreateEntry")]

        public async Task<IActionResult> CreateEntry([FromBody] CreateEntryCommand createEntryCommand)
        {
            //createdbyıd değeri gönderilmezse otomatik olarak istek yapan kullanıcının ıd si eklenecek 
            if (!createEntryCommand.CreatedById.HasValue)
                createEntryCommand.CreatedById = UserId;
            var result =await mediator.Send(createEntryCommand);

            return Ok(result);
        }


        [HttpPost]
        [Route("CreateEntryComment")]

        public async Task<IActionResult> CreateEntryComment([FromBody] CreateEntryCommentCommand createEntryCommentCommand)
        {
            if (!createEntryCommentCommand.CreatedById.HasValue)
                createEntryCommentCommand.CreatedById = UserId;

            var result = await mediator.Send(createEntryCommentCommand);

            return Ok(result);
        }
    }
}

using AutoMapper;
using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Common.Models.RequestModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Api.Application.Features.Commands.EntryComment.Create
{
    public class CreateEntryCommentCommandHandlerr : IRequestHandler<CreateEntryCommentCommand, Guid>
    {
        private readonly IEntryCommentRepository entryCommentRepository;

        private readonly IMapper mapper;

        public CreateEntryCommentCommandHandlerr(IEntryCommentRepository entryCommentRepository, IMapper mapper)
        {
            this.entryCommentRepository = entryCommentRepository;
            this.mapper = mapper;
        }

        public async Task<Guid> Handle(CreateEntryCommentCommand request, CancellationToken cancellationToken)
        {
            var dbEntryComment = mapper.Map<Domain.Models.EntryComment>(request);

            await entryCommentRepository.AddAsync(dbEntryComment);

            return dbEntryComment.Id;
        }
    }
}

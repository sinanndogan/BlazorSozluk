using AutoMapper;
using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Common;
using BlazorSozluk.Common.Events.User;
using BlazorSozluk.Common.Infrastructure;
using BlazorSozluk.Common.Infrastructure.Exceptions;
using BlazorSozluk.Common.Models.RequestModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Api.Application.Features.Commands.User.Create
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Guid>
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public CreateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            //Daha önce aynı email  adresiyle bir kullanıcı yaratılmıs mı onu kontrol ediyoruzz

            var existsUser = await userRepository.GetSingleAsync(i => i.EmailAddress == request.EmailAddress);

            if (existsUser is not null) {
                throw new DatabaseValidationException("User already exists!");
            }


            //Eğer kullanıcı kontrol sonrasında çıkmazsa yapılan işlem mapper'a diyoruz ki bana bir kullanıcı yarat ve bunu request objesinden al dönüştür

            var dbUser = mapper.Map<BlazorSozluk.Api.Domain.Models.User>(request);


            // bu kayıt veritabanına başarı ile kayıt edilip edilmediğini dönüyor 
            var rows = await userRepository.AddAsync(dbUser);

            //Email Changed/Created -- Burada email check yaptık
            if (rows > 0)
            {
                var @event = new UserEmailChangedEvent()
                {
                    OldEmailAddress = null,
                    NewEmailAddress = dbUser.EmailAddress
                };

                QueueFactory.SendMessageToExchange(exchangeName:SozlukConstants.UserExhangeName, exchangeType:SozlukConstants.DefaultExchangeType, queueName:SozlukConstants.UserEmailChangedQueueName,  obj:@event);
            }

           return dbUser.Id;

        }
    }
}

using AutoMapper;
using BlazorSozluk.Api.Application.Interfaces.Repositories;
using BlazorSozluk.Api.Domain.Models;
using BlazorSozluk.Common.Events.User;
using BlazorSozluk.Common.Infrastructure;
using BlazorSozluk.Common;
using BlazorSozluk.Common.Infrastructure.Exceptions;
using BlazorSozluk.Common.Models.RequestModels;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Api.Application.Features.Commands.User.Update
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Guid>
    {
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public UpdateUserCommandHandler(IUserRepository userRepository, IMapper mapper)
        {
            this.userRepository = userRepository;
            this.mapper = mapper;
        }

        public async Task<Guid> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            //Update edilecek kullanıcının sistemde olup olmadıgını kontrol etmemiz gerekiyor 

            var anyUser = await userRepository.GetByIdAsync(request.Id);

            if (anyUser == null)
            {
                throw new DatabaseValidationException("User not found!");
            }

            //email karşılaştırması yapılmakta
            var dbEmailAddress = anyUser.EmailAddress;
            var emailChanged = string.CompareOrdinal(dbEmailAddress, request.EmailAddress) != 0;


            #region ÖnemliNotMapping
            //Normalde bir mapleme olayını biz  anyUser= mapper.map<User>(request); şeklinde yapardık ama böyle yaptıgımız zaman 
            //yeni bir kullanıcı oluşturmus oluyoruz 
            //ama bize yaratması değil var olana ekleme yani değişim yapmasını istiyoruz bundan dolayı mapperin diğer mmetodu olan map fonksiyonunu kullannmamız gerekcektir  map fonksiyonu bizden bir kaynak bir de çıktı isteyecek mapper.Map(request, anyUser); bu bizim işimizi gören kod olacaktır.
            #endregion


            mapper.Map(request, anyUser);

            var rows = await userRepository.UpdateAsync(anyUser);


            //Check if email changed Eğer kullanıcı email'i değişmişse biziim yeni kullanıcıya notification emaili göndermemmiz lazım

            if (rows > 0)
            {
                var @event = new UserEmailChangedEvent()
                {
                    OldEmailAddress = null,
                    NewEmailAddress = anyUser.EmailAddress
                };

                QueueFactory.SendMessageToExchange(exchangeName: SozlukConstants.UserExhangeName, exchangeType: SozlukConstants.DefaultExchangeType, queueName: SozlukConstants.UserEmailChangedQueueName, obj: @event);

                ///email değiştiği için confirm edilmeli o yüzden default olarak false atandı email yapısınaa
                anyUser.EmailConfirmed = false;
                await userRepository.UpdateAsync(anyUser);
            }


            return anyUser.Id;
        }
    }
}

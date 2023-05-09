using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorSozluk.Common
{
    public class SozlukConstants
    {
        public const string RabbitMQHost = "amqps://vgoeohfm:NmkSmsxycy31BqcSuJ7Me8qFIdR0E6Y_@shrimp.rmq.cloudamqp.com/vgoeohfm";
        public const string DefaultExchangeType = "direct";



        public const string UserExhangeName = "UserExchange";

        public const string UserEmailChangedQueueName = "UserEmailChangedQueue";




        public const string FavExchangeName = "FavExchange";

        public const string CreateEntryCommentFavQueueName = "CreateEntryCommentFavQueue";
}  }

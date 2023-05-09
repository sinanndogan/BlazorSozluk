using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace BlazorSozluk.Common.Infrastructure
{
    public static  class QueueFactory
    {
        public static void SendMessageToExchange(string exchangeName, string exchangeType, string queueName, object obj)
        {
            var channel = CreateBasicConsumer()
                   .EnsureExchange(exchangeName, exchangeType)
                   .EnsureQueue(queueName, queueName)
                   .Model;

            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj));

            channel.BasicPublish(exchange:exchangeName, routingKey:queueName, basicProperties:null,body:body);
        }


        public static EventingBasicConsumer CreateBasicConsumer()
        {
            var factory = new ConnectionFactory()
            {
                Uri = new Uri(SozlukConstants.RabbitMQHost)
            };
            var connection = factory.CreateConnection();
            
            var channael = connection.CreateModel();

            return new EventingBasicConsumer(channael);
        }


        public static EventingBasicConsumer EnsureExchange(this EventingBasicConsumer consumer, string exchangeName, string exchangeType = SozlukConstants.DefaultExchangeType)
        {
            consumer.Model.ExchangeDeclare( exchange:exchangeName, type: exchangeType, durable: false, autoDelete: false);
            return consumer;
        }


        public static EventingBasicConsumer EnsureQueue(this EventingBasicConsumer consumer, string queueName, string exchangeName = SozlukConstants.DefaultExchangeType)
        {
            consumer.Model.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete:false, null);

            consumer.Model.QueueUnbind(queueName,exchangeName,queueName);

            
            return consumer;
        }


    }
}

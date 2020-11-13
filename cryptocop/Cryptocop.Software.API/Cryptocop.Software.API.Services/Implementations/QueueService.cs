using Cryptocop.Software.API.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using Cryptocop.Software.API.Services.Helpers;
using System.Text;
using Newtonsoft.Json.Serialization;

namespace Cryptocop.Software.API.Services.Implementations
{
    public class QueueService : IQueueService, IDisposable
    {
        private readonly IConnection _connection;
        private IModel _channel;
        private IConfiguration Configuration;
        private readonly string _routingKey;

        public QueueService(IConfiguration configuration)
        {
            _routingKey = configuration.GetSection("MessageBroker").GetSection("RoutingKey").Value;
            Configuration = configuration;
            try
            {
                var messageBrokerSection = configuration.GetSection("MessageBroker");
                var factory = new ConnectionFactory();
                factory.Uri = new Uri(messageBrokerSection.GetSection("ConnectionString").Value);
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
            }
            catch
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
            }
        }

        public void PublishMessage(string routingKey, object body)
        {
            // Publish the message using a channel created with
            // the RabbitMQ client
            var messageBrokerSection = Configuration.GetSection("MessageBroker");
            _channel.BasicPublish(
                exchange: messageBrokerSection.GetSection("ExchangeName").Value,
                routingKey,
                mandatory: true,
                basicProperties: null,
                body: ConvertJsonToBytes(body));
        }

        private byte[] ConvertJsonToBytes(object obj) => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));

        public void Dispose()
        {
            _channel.Close();
            _connection.Close();
        }
    }
}
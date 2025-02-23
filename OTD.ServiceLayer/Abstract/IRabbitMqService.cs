namespace OTD.ServiceLayer.Abstract
{
    public interface IRabbitMqService
    {
        void Publish<T>(string queueName, T message);
        void Consume(string queueName, Action<string> onMessageReceived);
    }
}

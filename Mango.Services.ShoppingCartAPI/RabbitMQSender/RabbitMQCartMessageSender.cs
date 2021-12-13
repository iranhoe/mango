﻿namespace Mango.Services.ShoppingCartAPI.RabbitMQSender;

using System.Text;
using System.Text.Json;
using MessageBus;
using RabbitMQ.Client;

public class RabbitMQCartMessageSender : IRabbitMQCartMessageSender
{
    private readonly string _hostname;
    private readonly string _password;
    private readonly string _username;
    private IConnection _connection;

    public RabbitMQCartMessageSender()
    {
        _hostname = "localhost";
        _password = "guest";
        _username = "guest";
    }
    
    public void SendMessage(BaseMessage message, string queueName)
    {
        var factory = new ConnectionFactory
        {
            HostName = _hostname,
            UserName = _username,
            Password = _password
        };
        _connection = factory.CreateConnection();
        using var channel = _connection.CreateModel();
        channel.QueueDeclare(queue: queueName, false, false, false, arguments: null);
        var json = JsonSerializer.Serialize(message, message.GetType());
        var body = Encoding.UTF8.GetBytes(json);
        channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties:null, body: body);
    }
}
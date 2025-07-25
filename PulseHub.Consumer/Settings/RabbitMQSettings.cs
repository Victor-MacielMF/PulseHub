﻿using PulseHub.Domain.Messaging;

namespace PulseHub.Consumer.Settings
{
    public class RabbitMQSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string QueueName { get; set; }
    }
}

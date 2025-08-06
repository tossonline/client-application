// Copyright (c) DigiOutsource. All rights reserved.

namespace Analytics.Domain.Models.Configuration.Kafka
{
    [Serializable]
    public class KafkaProducerConfiguration
    {
        public KafkaProducerConfiguration()
        {
            Brokers = new List<string>();
            Topic = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            Serializer = string.Empty;
            SchemaRegistryUrl = string.Empty;
        }

        public List<string> Brokers { get; set; }

        public string Topic { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Serializer { get; set; }

        public string SchemaRegistryUrl { get; set; }
    }
}
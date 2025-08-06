// Copyright (c) DigiOutsource. All rights reserved.

namespace Analytics.Domain.Models.Configuration.Kafka
{
    [Serializable]
    public sealed class KafkaConsumerConfiguration
    {
        public KafkaConsumerConfiguration()
        {
            Brokers = new List<string>();
            Topics = new List<string>();
            GroupId = string.Empty;
            Username = string.Empty;
            Password = string.Empty;
            Deserializer = string.Empty;
            SchemaRegistryUrl = string.Empty;
        }

        public List<string> Brokers { get; set; }

        public List<string> Topics { get; set; }

        public string GroupId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Deserializer { get; set; }

        public string SchemaRegistryUrl { get; set; }
    }
}
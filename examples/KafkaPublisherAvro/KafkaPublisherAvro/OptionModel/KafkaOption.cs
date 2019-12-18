using System.Collections.Generic;

namespace KafkaPublisherAvro.OptionModel
{
    public class KafkaOption
    {
        public KafkaServer Servers { get; set; }
        public List<KafkaSubscription> Subscriptions { get; set; }
        public List<KafkaPublisher> Publishers { get; set; }
        public string GroupId { get; set; }
        public string Protocol { get; set; }
    }

    public class KafkaServer
    {
        public List<KafkaBroker> Brokers { get; set; }
        public SchemaRegistry SchemaRegistry { get; set; }
    }


    public class KafkaBroker
    {
        public string PublicIp { get; set; }
        public List<string> Ports { get; set; }
    }

    public class SchemaRegistry
    {
        public string PublicIp { get; set; }
        public string Port { get; set; }
    }

    public class KafkaSubscription
    {
        public string Topic { get; set; }
        public string Partition { get; set; }
        public string ServiceName { get; set; }
        public string MethodName { get; set; }
    }

    public class KafkaPublisher
    {
        public string Topic { get; set; }
        public string ServiceName { get; set; }
    }
}
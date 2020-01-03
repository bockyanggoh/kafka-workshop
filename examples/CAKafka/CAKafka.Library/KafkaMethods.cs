using CAKafka.Domain.Models;

namespace CAKafka.Library
{
    public class KafkaMethods
    {
        public static string GenerateKafkaBrokerString(KafkaOptions options)
        {
            var bootstrapServers = "";
            foreach (KafkaBroker k in options.Servers.Brokers)
            {
                foreach (var port in k.Ports)
                {
                    bootstrapServers += $"{k.PublicIp}:{port},";
                }
            }
            if (bootstrapServers.EndsWith(","))
            {
                bootstrapServers = bootstrapServers.Substring(0, bootstrapServers.Length - 1);
            }

            return bootstrapServers;
        }
    }
}
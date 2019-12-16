# Kafka Workshop

## Useful Kafka CLI Commands:

### List Topics on Brokers 
`kafka-topics.sh --list --bootstrap-server kafka1:9092`

### Create Topic via Broker 
`kafka-topics.sh --create --bootstrap-server kafka1:9092 --replication-factor 3 --partitions 3 --topic test`

### Create Topic via ZooKeeper 
`kafka-topics.sh --create —zookeeper kafka1:2181 --replication-factor 3 --partitions 3 --topic test`

### Delete Topic 
`kafka-topics.sh —zookeeper kafka1:2181 —delete —topic `

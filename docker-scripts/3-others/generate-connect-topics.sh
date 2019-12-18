kafka-topics.sh --create --replication-factor 3 --partitions 3 --topic SuperAvro --zookeeper kafka1:22181
kafka-topics.sh --create --replication-factor 3 --partitions 3 --topic SuperEasy --zookeeper kafka1:22181

kafka-topics.sh --create --replication-factor 1 --partitions 1 --topic workshop-sql-avro-offsets --zookeeper kafka1:22181
kafka-topics.sh --create --replication-factor 1 --partitions 1 --topic workshop-sql-avro-config --zookeeper kafka1:22181
kafka-topics.sh --create --replication-factor 1 --partitions 1 --topic workshop-sql-avro-status --zookeeper kafka1:22181

kafka-topics.sh --create --replication-factor 1 --partitions 1 --topic workshop-loki-avro-offsets --zookeeper kafka1:22181
kafka-topics.sh --create --replication-factor 1 --partitions 1 --topic workshop-loki-config --zookeeper kafka1:22181
kafka-topics.sh --create --replication-factor 1 --partitions 1 --topic workshop-loki-status --zookeeper kafka1:22181

kafka-topics.sh --create --replication-factor 1 --partitions 1 --topic workshop-azure-offsets --zookeeper kafka1:22181
kafka-topics.sh --create --replication-factor 1 --partitions 1 --topic workshop-azure-config --zookeeper kafka1:22181
kafka-topics.sh --create --replication-factor 1 --partitions 1 --topic workshop-azure-status --zookeeper kafka1:22181

kafka-topics.sh --describe --zookeeper kafka1:22181

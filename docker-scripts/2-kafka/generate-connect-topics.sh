kafka-topics.sh --create --replication-factor 2 --partitions 2 --topic dead --zookeeper kafka1:22181

kafka-topics.sh --create --replication-factor 3 --partitions 3 --topic SuperAvroSpecific --zookeeper kafka1:22181
kafka-topics.sh --create --replication-factor 3 --partitions 3 --topic SuperAvroGeneric --zookeeper kafka1:22181
kafka-topics.sh --create --replication-factor 3 --partitions 3 --topic SuperEasyJson --zookeeper kafka1:22181
kafka-topics.sh --create --replication-factor 3 --partitions 3 --topic SuperEasy --zookeeper kafka1:22181

kafka-topics.sh --create --replication-factor 1 --partitions 1 --topic workshop-sql-avro-offsets --zookeeper kafka1:22181
kafka-topics.sh --create --replication-factor 1 --partitions 1 --topic workshop-sql-avro-config --zookeeper kafka1:22181
kafka-topics.sh --create --replication-factor 1 --partitions 1 --topic workshop-sql-avro-status --zookeeper kafka1:22181

kafka-topics.sh --create --replication-factor 2 --partitions 2 --topic PaymentRequestAvro --zookeeper kafka1:22181
kafka-topics.sh --create --replication-factor 2 --partitions 2 --topic PaymentRequestJson --zookeeper kafka1:22181
kafka-topics.sh --zookeeper kafka1:22181 --alter --topic PaymentRequestAvro --config retention.ms=30000
kafka-topics.sh --zookeeper kafka1:22181 --alter --topic PaymentRequestJson --config retention.ms=30000

kafka-topics.sh --create --replication-factor 2 --partitions 2 --topic OrderPaymentStatusUpdateJson --zookeeper kafka1:22181
kafka-topics.sh --create --replication-factor 2 --partitions 2 --topic OrderPaymentStatusUpdateAvro --zookeeper kafka1:22181
kafka-topics.sh --zookeeper kafka1:22181 --alter --topic OrderPaymentStatusUpdateJson --config retention.ms=30000
kafka-topics.sh --zookeeper kafka1:22181 --alter --topic OrderPaymentStatusUpdateAvro --config retention.ms=30000

kafka-topics.sh --describe --zookeeper kafka1:22181

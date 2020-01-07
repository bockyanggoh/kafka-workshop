kafka1=$(az vm show -d -g 'kafka-rg' -n kafka1 --query publicIps -o tsv)
kafka2=$(az vm show -d -g 'kafka-rg' -n kafka2 --query publicIps -o tsv)
echo "Kafka 1 Public IP: $kafka1"
echo "Kafka 2 Public IP: $kafka2"
echo "SQL Database Public IP: $kafka2, Access Port: 1433"
echo "Schema Registry Access: http://$kafka2:8081"
echo "KafDrop Access: http://$kafka2:9000"
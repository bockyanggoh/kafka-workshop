#!/bin/bash
az group create -l 'southeast asia' -n 'kafka-rg'
az group deployment create --resource-group 'kafka-rg' --template-file azure-provision/kafka-vm-deploy.json --parameters azure-provision/kafka-vm-parameters.json

echo "Running Post-Installation scripts"
nohup az vm run-command invoke -g 'kafka-rg' -n 'kafka1' --command-id RunShellScript --scripts @azure-provision/postinstall-scripts/install.sh &
az vm run-command invoke -g 'kafka-rg' -n 'kafka2' --command-id RunShellScript --scripts @azure-provision/postinstall-scripts/install.sh

echo "Mission Complete. Keep the following information!"
kafka1=$(az vm show -d -g 'kafka-rg' -n kafka1 --query publicIps -o tsv)
kafka2=$(az vm show -d -g 'kafka-rg' -n kafka2 --query publicIps -o tsv)
echo "Kafka 1 Public IP: $kafka1"
echo "Kafka 2 Public IP: $kafka2"
echo "SQL Database Public IP: $kafka2, Access Port: 1433"

echo "Connect to Kafka1 with this command: 'ssh kafka@$kafka1'"
echo "Connect to Kafka2 with this command: 'ssh kafka@$kafka2'"
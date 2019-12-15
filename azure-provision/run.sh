#!/bin/bash
az group create -l 'southeast asia' -n 'kafka-rg'
az group deployment create --resource-group 'kafka-rg' --template-file azure-provision/kafka-vm-deploy.json --parameters azure-provision/kafka-vm-parameters.json

sleep 5s

nohup az vm run-command invoke -g 'kafka-rg' -n 'kafka1' --command-id RunShellScript --scripts @azure-provision/postinstall-scripts/install.sh &
az vm run-command invoke -g 'kafka-rg' -n 'kafka2' --command-id RunShellScript --scripts @azure-provision/postinstall-scripts/install.sh

echo "Mission Complete. Keep the following information!"
echo "Kafka 1 Public IP: $(az vm show -d -g 'kafka-rg' -n kafka1 --query publicIps -o tsv)"
echo "Kafka 2 Public IP: $(az vm show -d -g 'kafka-rg' -n kafka2 --query publicIps -o tsv)"
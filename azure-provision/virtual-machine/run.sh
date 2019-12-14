#!/bin/bash

#az group create -l 'southeast asia' -n 'kafka-rg'
az group deployment create --resource-group 'kafka-rg' --template-file kafka-vm-deploy.json --parameters kafka-vm-parameters.json
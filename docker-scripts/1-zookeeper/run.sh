#!/bin/bash

compose_file="$1"
hostName=$(hostname)

if [ -z "$hostName" ]; then
  exit 1
fi
if [ $hostName == "kafka1" ]; then
  if [ $compose_file == "server2.yml" ]; then
    echo "Wrong server file used. Please use the right file."
    exit 10
  fi
  export ZK_O_HOSTNAME="kafka2"
else {
  if [ $compose_file == "server1.yml" ]; then
    echo "Wrong server file used. Please use the right file."
    exit 10
  fi
  export ZK_O_HOSTNAME="kafka1"
}
fi
echo "ZK_O_HOSTNAME is set to $ZK_O_HOSTNAME"

docker-compose -f $compose_file down
docker-compose -f $compose_file up -d

docker-compose -f $compose_file logs -f

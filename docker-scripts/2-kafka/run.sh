#!/bin/bash
compose_file="$1"
hostName=$(hostname)
export HOSTNAME=$(hostname)
export PUBLIC_IP="$(host myip.opendns.com resolver1.opendns.com | grep "myip.opendns.com has" | awk '{print $4}')"

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

echo "ZK_O_HOSTNAME=$ZK_O_HOSTNAME, PUBLIC_IP=$PUBLIC_IP"

docker-compose -f $compose_file down
docker-compose -f $compose_file up -d

docker-compose -f $compose_file logs -f
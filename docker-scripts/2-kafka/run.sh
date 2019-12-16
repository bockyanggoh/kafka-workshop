#!/bin/bash
compose_file="$1"
other_ip="$2"
public_ip="$3"
export HOSTNAME=$(hostname)
export ZK_O_HOSTNAME=$other_ip
export PUBLIC_IP=$public_ip
echo "ZK_O_HOSTNAME=$ZK_O_HOSTNAME, PUBLIC_IP=$PUBLIC_IP"

docker-compose -f $compose_file down
docker-compose -f $compose_file up -d

docker-compose -f $compose_file logs -f
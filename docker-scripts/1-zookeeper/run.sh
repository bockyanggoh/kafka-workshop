#!/bin/bash

compose_file="$1"
other_ip="$2"
docker-compose -f $compose_file down
export ZK_O_HOSTNAME=$other_ip

echo "ZK_O_HOSTNAME is set to $ZK_O_HOSTNAME"
docker-compose -f $compose_file up -d



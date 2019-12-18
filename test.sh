#!/bin/bash

hostName=$(hostname)

if [ -z "$hostName" ]; then
  exit 1
fi
if [ $hostName == "Bock-MBP.local" ]; then
  hostName="kafka2"
else {
  hostName="kafka1"
}
fi

echo $hostName
# Zookeeper Cluster
This section contains the required files for starting up zookeeper on 2 VMs, 5 instances of zookeeper.


## Run Instructions
You should SSH into either kafka1 or kafka2 of your kafka servers. Once there, cd to this folder, issue the corresponding script for your server.
Step 1: CD Command
```bash
cd /workspace/kafka-workshop/docker-scripts/1-zookeeper
```
Step 2: For kafka1,
```bash
./run.sh server1.yml
```
Step 2: For kafka2,
```bash
./run.sh server1.yml
```
Your server should now be up and running!

## Explanation
[run.sh](run.sh) is your entrypoint to kafka.
1. ZK_O_HOSTNAME is the hostname of the other kafka server. run file will detect your hostname and set the other hostname for you.(eg. kafka1 is your host, ZK_O_HOSTNAME will be kafka2)
2. After deciphering that, it will try to run your docker-compose detached and show you the logs of the docker-compose. This means it's safe to exit out of the logs without taking down the compose instances.

[Zookeeper Config](server1.yml) is your configuration of zookeeper
To view the full list of possible zookeeper configs: Refer to [this]: https://zookeeper.apache.org/doc/r3.3.3/zookeeperAdmin.html#sc_configuration


### *Important Note*
For configurations in docker, Confluent/Apache implements this format: ZOOKEEPER_CONFIGNAME. This applies to all configs. For true/false values, it must be encapsulated in "" or an error will occur.

For Example: dataDir
In docker, the config will be **ZOOKEEPER_DATA_DIR**



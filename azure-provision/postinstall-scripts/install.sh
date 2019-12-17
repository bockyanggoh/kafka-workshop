# Install useful tools for networking checks
sudo yum update -y
sudo yum install nc -y
sudo yum install git -y
sudo yum install https://dl.fedoraproject.org/pub/epel/epel-release-latest-7.noarch.rpm -y
sudo yum install htop -y
sudo yum install java-11-openjdk-devel -y
# Install Docker
curl -fsSL https://get.docker.com/ | sh
sudo systemctl start docker
sudo systemctl status docker
sudo systemctl enable docker


# Post install Sudo
sudo usermod -aG docker kafka

# Install Docker-Compose
sudo curl -L "https://github.com/docker/compose/releases/download/1.25.0/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose
sudo ln -s /usr/local/bin/docker-compose /usr/bin/docker-compose

# Set Home Directory
mkdir /workspace
chown kafka:user /workspace

# Install Kafka CLI
wget --directory-prefix=/workspace/ https://www-eu.apache.org/dist/kafka/2.3.0/kafka_2.12-2.3.0.tgz
tar -C /workspace -xzf /workspace/kafka_2.12-2.3.0.tgz
echo "export PATH=$PATH:/workspace/kafka_2.12-2.3.0/bin" > /etc/profile.d/kafka-path.sh
echo "source /workspace/.bashrc" >> /etc/profile.d/kafka-path.sh
chmod 777 /etc/profile.d/kafka-path.sh
chown root:root /etc/profile.d/kafka-path.sh

rm -f /workspace/kafka_2.12-2.3.0.tgz

# Download Kafka Workshop files
git clone https://github.com/bockyanggoh/kafka-workshop.git /workspace/kafka-workshop
find /workspace/kafka-workshop -name "*.sh" -exec chmod +x {} \;
cp /home/kafka/.bash* /workspace/
# Set new home
usermod -m -d /workspace kafka
chown -R kafka:kafka /workspace
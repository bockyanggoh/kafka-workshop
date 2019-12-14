# Install useful tools for networking checks
sudo yum check-update
sudo yum install nc -y
sudo yum install git -y
# Install Docker
curl -fsSL https://get.docker.com/ | sh
sudo systemctl start docker
sudo systemctl status docker
sudo systemctl enable docker

# Post install Sudo
sudo usermod -aG docker $(whoami)

# Install Docker-Compose
sudo curl -L "https://github.com/docker/compose/releases/download/1.25.0/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose
sudo ln -s /usr/local/bin/docker-compose /usr/bin/docker-compose
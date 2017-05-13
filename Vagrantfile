Vagrant.configure("2") do |config|
  config.ssh.host = "127.0.0.1"
  config.ssh.username = "vagrant"
  config.ssh.password = "vagrant"
  config.vm.box = "ubuntu/trusty64"
  config.vm.box_check_update = false
  config.vm.hostname = "docker"
  config.vm.network :forwarded_port, id: 'ssh', guest: 22, host: 2200, auto_correct: false
  config.vm.network :forwarded_port, id: 'rmq-epmd', guest: 4369, host: 4369, auto_correct: false
  config.vm.network :forwarded_port, id: 'rmq-amqpWithTls', guest: 5671, host: 5671, auto_correct: false
  config.vm.network :forwarded_port, id: 'rmq-amqpWithoutTls', guest: 5672, host: 5672, auto_correct: false
  config.vm.network :forwarded_port, id: 'rmq-management1', guest: 15671, host: 15671, auto_correct: false
  config.vm.network :forwarded_port, id: 'rmq-management2', guest: 15672, host: 15672, auto_correct: false
  config.vm.network :forwarded_port, id: 'rmq-erlangDistribution', guest: 25672, host: 25672, auto_correct: false
  config.vm.network :forwarded_port, id: 'elastic-rest', guest: 9200, host: 9200, auto_correct: false
  config.vm.network :forwarded_port, id: 'elastic-nodeCommunication', guest: 9300, host: 9300, auto_correct: false
  config.vm.network :forwarded_port, id: 'kibana', guest: 5601, host: 5601, auto_correct: false
  config.vm.provider "virtualbox" do |vb|
    vb.name = "vm_docker"
    vb.cpus = 2
    vb.memory = 3072
  end
  config.vm.provision "shell", inline: <<-SHELL.gsub(/^ +/, '')
    echo "****************************** Configuring system settings"
    sudo timedatectl set-timezone Europe/Rome
    sudo update-locale LANG=en_US.UTF-8 LC_ALL=en_US.UTF-8
    echo "%sudo ALL=(ALL) NOPASSWD: ALL" >> /etc/sudoers
    echo "****************************** Downloading packages"
    sudo aptitude -q -y update
    sudo aptitude -q -y upgrade
    sudo aptitude -q -y dist-upgrade
    sudo aptitude -q -y install sysv-rc-conf sudo linux-kernel-headers git tree whois unzip nginx
    echo "****************************** Updating Guest Additions"
    sudo wget -q -c http://download.virtualbox.org/virtualbox/LATEST.TXT -O /tmp/vbga-latest-version.txt
    vbgaLatestVersion=$(sudo cat /tmp/vbga-latest-version.txt)
    sudo rm /tmp/vbga-latest-version.txt
    sudo wget -q -c http://download.virtualbox.org/virtualbox/$vbgaLatestVersion/VBoxGuestAdditions_$vbgaLatestVersion.iso -O vbga.iso
    sudo mount vbga.iso -o loop /mnt
    sudo sh /mnt/VBoxLinuxAdditions.run -- force
    sudo umount /mnt
    sudo rm vbga.iso
    sudo sysv-rc-conf vboxadd on
    echo "****************************** Installing Docker"
    if ! docker version &>/dev/null; then
      echo "*** Installing Docker Engine"
      wget -qO- https://get.docker.com/ | sudo bash
      sudo usermod -aG docker vagrant
    fi
    if ! docker-compose --version &>/dev/null; then
      echo "*** Installing Docker Compose"
      sudo wget -q -c https://github.com/docker/compose/releases/download/1.13.0/docker-compose-`uname -s`-`uname -m` -O /usr/local/bin/docker-compose
      sudo chmod +x /usr/local/bin/docker-compose
    fi
  SHELL
end
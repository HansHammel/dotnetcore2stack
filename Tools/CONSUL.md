TODO: https://www.consul.io/api/index.html
https://www.digitalocean.com/community/tutorials/how-to-secure-consul-with-tls-encryption-on-ubuntu-14-04
-> https://www.digitalocean.com/community/tutorials/how-to-configure-consul-in-a-production-environment-on-ubuntu-14-04

-> http://blog.medhat.ca/2016/10/codefirst-with-net-core-console_1.html

https://github.com/hashicorp/consul-template

https://github.com/Drawaes/CondenserDotNet
https://github.com/PlayFab/consuldotnet

Install-Package DnsClient 
dotnet add package DnsClient 
Install-Package Consul 
dotnet add package Consul

https://releases.hashicorp.com/consul-template/0.19.4/consul-template_0.19.4_windows_amd64.zip
https://releases.hashicorp.com/consul-template/0.19.4/consul-template_0.19.4_linux_amd64.zip
https://releases.hashicorp.com/consul-template/0.19.4/consul-template_0.19.4_darwin_amd64.zip

http://www.mammatustech.com/consul-service-discovery-and-health-for-microservices-architecture-tutorial

	consul keygen
	consul agent -ui

	// run consule in dev mode
	consul agent -dev -ui
	// consul -> localhost:8300
	// DNS -> localhost:8600
	// UI at http://127.0.0.1:8500/ui
	// show current members
	consul members
	// using HTTP API
	curl localhost:8500/v1/catalog/nodes

	consul agent -dev -ui -config-dir=./etc/consul.d -data-dir=./etc/consul-data -enable-script-checks
	curl http://localhost:8500/v1/catalog/service/web

	//clustering 2 nodes
	consul agent -server -bootstrap-expect=1 -data-dir=/tmp/consul -node=agent-one -bind=172.20.20.10 -enable-script-checks=true -config-dir=/etc/consul.d
	consul agent -data-dir=/tmp/consul -node=agent-two -bind=172.20.20.11 -enable-script-checks=true -config-dir=/etc/consul.d
	//tell first agent to join secound
	consul join 172.20.20.11
	//reloading a config
	consul reload

	//using key value store
	consul kv put redis/config/minconns 1
	consul kv get redis/config/minconns
	consul kv put -flags=42 redis/config/users/admin abcd1234
	consul kv get -detailed redis/config/users/admin
	consul kv get -recurse
	consul kv delete redis/config/minconns
	consul kv delete -recurse redis
    curl --request PUT --data 'hello consul' http://localhost:8500/v1/kv/foo
	curl --request GET http://localhost:8500/v1/kv/foo
	// configuration of the agent
	curl http://localhost:8500/v1/agent/self
	// reload config
	curl --request PUT http://localhost:8500/v1/agent/reload
	// maintenance mode
	curl --request PUT http://localhost:8500/v1/agent/maintenance?enable=true&reason=For+API+docs
	curl --request PUT http://localhost:8500/v1/agent/metrics
	curl --request PUT http://localhost:8500/v1/agent/monitor
	curl --request PUT http://localhost:8500/v1/agent/join/10.0.0.1
	curl --request PUT http://localhost:8500/v1/agent/leave
	curl --request PUT http://localhost:8500/v1/agent/force-leave
	curl --request PUT --data @payload.json http://localhost:8500/v1/agent/token/acl_token
	curl http://localhost:8500/v1/agent/services
	curl --request PUT --data @payload.json http://localhost:8500/v1/agent/service/register
	curl --request PUT http://localhost:8500/v1/agent/service/deregister/my-service-id
	curl --request PUT http://localhost:8500/v1/agent/service/maintenance/my-service-id?enable=true&reason=For+the+docs

	//check for failing helth checks
	curl http://localhost:8500/v1/health/state/critical
	curl http://localhost:8500/v1/health/state/warning
	curl http://localhost:8500/v1/health/state/passing

	curl http://localhost:8500/v1/catalog/services

# another .travis.yml

	sudo: false
	env:
	  global:
		- CONSUL_VERSION=0.5.0
		- CONSUL_DC=dev1
		- CONSUL_DIR=$HOME/consul_$CONSUL_VERSION

	before_script:
	  - 'if [[ ! -f $CONSUL_DIR/consul ]]; then (mkdir -p $CONSUL_DIR && cd $CONSUL_DIR && wget https://dl.bintray.com/mitchellh/consul/${CONSUL_VERSION}_linux_amd64.zip && unzip ${CONSUL_VERSION}_linux_amd64.zip); fi'
	  - $CONSUL_DIR/consul --version
	  - $CONSUL_DIR/consul agent -server -bootstrap-expect 1 -data-dir /tmp/consul -dc=$CONSUL_DC &
	  # Wait for consul to elect itself as leader
	  - sleep 5

	cache:
	  directories:
		- $CONSUL_DIR
		
		
		
language: generic

addons:
  apt:
    packages:
    - gettext
    - libcurl4-openssl-dev
    - libicu-dev
    - libssl-dev
    - libunwind8
    - zlib1g

matrix:
  include:
    - os: linux
      dist: trusty
      sudo: required

before_install:
  # Install OpenSSL
  - if test "$TRAVIS_OS_NAME" == "osx"; then
      brew install openssl;
      brew link --force openssl;
      export DOTNET_SDK_URL="https://go.microsoft.com/fwlink/?linkid=843444";
    else
      export DOTNET_SDK_URL="https://go.microsoft.com/fwlink/?linkid=843450";
    fi

  - export DOTNET_INSTALL_DIR="$PWD/.dotnetcli"

  # Install .NET CLI
  - mkdir $DOTNET_INSTALL_DIR
  - curl -L $DOTNET_SDK_URL -o dotnet_package
  - tar -xvzf dotnet_package -C $DOTNET_INSTALL_DIR

  # Add dotnet to PATH
  - export PATH="$DOTNET_INSTALL_DIR:$PATH"
  
install:
  # Display dotnet version info
  - which dotnet;
    if [ $? -eq 0 ]; then
      echo "Using dotnet:";
      dotnet --info;
    else
      echo "dotnet.exe not found"
      exit 1;
    fi
  # Restore dependencies
  - dotnet restore

before_script:
  # Install consul
  - if test "$TRAVIS_OS_NAME" == "osx"; then
      wget 'https://releases.hashicorp.com/consul/0.7.0/consul_0.7.0_darwin_amd64.zip';
      unzip "consul_0.7.0_darwin_amd64.zip";
    else
      wget 'https://releases.hashicorp.com/consul/0.7.0/consul_0.7.0_linux_amd64.zip';
      unzip "consul_0.7.0_linux_amd64.zip";
    fi
  - ./consul --version

script:
  # Start Consul
  - ./consul agent -server -bootstrap-expect 1 -log-level err -data-dir /tmp/consul -advertise=127.0.0.1 &
  # Build sample
  - dotnet test test/CondenserTests/CondenserTests.csproj
  - dotnet test test/Condenser.Tests.Integration/Condenser.Tests.Integration.csproj



appveyor.yml

os: Visual Studio 2017
build: off

environment:
  sonarkey:
    secure: dqF6V11A7InHKcyOX6WDGE3oA54yZQm0r9VLio85ndCn2B8d9zVI2mJ3lQdDzO3o
  COVERALLS_REPO_TOKEN:
    secure: x41DSerLXKgGVbKIokF+zuR3eNRVJXsgJA6j5yggnCB8/TTyYfa/2euNflfGzCot   

install:
  - cmd: curl -fsS -o consul.zip https://releases.hashicorp.com/consul/0.7.5/consul_0.7.5_windows_amd64.zip
  - cmd: 7z x consul.zip -o"C:\Consul" -y > nul
  - ps: $MyProcess = Start-Process C:\Consul\consul.exe -ArgumentList "agent -server -log-level err -bootstrap-expect 1 -data-dir C:\Consul\Data -advertise=127.0.0.1" -PassThru

before_test:
  - ECHO %APPVEYOR_REPO_COMMIT_MESSAGE%
  - dotnet --info
  - VersionNumber.bat
  - dotnet restore
  
test_script:
  # Build sample
  - dotnet test test/CondenserTests/CondenserTests.csproj
  - dotnet test test/Condenser.Tests.Integration/Condenser.Tests.Integration.csproj
  
after_test:
  # Build and pack source
  - ps: iex ((Get-ChildItem ($env:USERPROFILE + '\.nuget\packages\OpenCover'))[0].FullName + '\tools\OpenCover.Console.exe' + ' -register:user -target:".\script\runtests.bat" -searchdirs:"..\test\Condenser.Tests.Integration\bin\Debug\netcoreapp1.1;..\test\CondenserTests\bin\debug\netcoreapp1.1" -oldstyle -output:coverage.xml -skipautoprops -hideskipped:All -returntargetcode -filter:"+[Condenser*]* -[*Test*]*"')
  - ps: iex ((Get-ChildItem ($env:USERPROFILE + '\.nuget\packages\coveralls.io'))[0].FullName + '\tools\coveralls.net.exe' + ' --opencover coverage.xml')
  - "SET PATH=C:\\Python34;C:\\Python34\\Scripts;%PATH%"
  - pip install codecov
  - codecov -f "coverage.xml"
  - dotnet build -c Release
  - dotnet pack -c Release src/CondenserDotNet.Client --version-suffix %suffix%
  - dotnet pack -c Release src/CondenserDotNet.Server --version-suffix %suffix%
  - dotnet pack -c Release src/CondenserDotNet.Core --version-suffix %suffix%
  - dotnet pack -c Release src/CondenserDotNet.Configuration --version-suffix %suffix%
  - dotnet pack -c Release src/CondenserDotNet.Middleware --version-suffix %suffix%
  - dotnet pack -c Release src/CondenserDotNet.Server.Extensions --version-suffix %suffix%
      
  
artifacts:
  - path: '**/*.nupkg'
    name: packages
  - path: 'coverage.xml'
    name: coverage
    type: zip

deploy:  
- provider: NuGet
  server: https://www.myget.org/F/condenserdotnet/api/v2/package
  api_key:
    secure: 5mBb0A2rlwk1Iq6FEo94XSORm9etc3xPn0oLZ8dIJ6Hmm1G7quqf+Bz6fm+ft+FK
  skip_symbols: true
  on:
    branch: master
  
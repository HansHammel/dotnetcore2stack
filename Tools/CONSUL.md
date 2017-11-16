TODO: https://www.consul.io/api/index.html
https://www.digitalocean.com/community/tutorials/how-to-secure-consul-with-tls-encryption-on-ubuntu-14-04

	consul keygen
	consul agent -ui

	// run consule in dev mode
	consul agent -dev -ui
	// consul -> localhost:8300
	// DNS -> localhost:8600
	// UI at http://localhost:8500/ui
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
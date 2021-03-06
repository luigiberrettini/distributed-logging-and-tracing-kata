#!/bin/bash

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )


printf "***** Stopping containers...\n"
docker stop ls2ela ls2rmq kbana elast rabmq

printf "\n***** Removing containers...\n"
docker rm ls2ela ls2rmq kbana elast rabmq

printf "\n***** Removing network...\n"
docker network rm bridgenet


if [ ! -d "$SCRIPT_DIR/network_vis" ]; then
    printf "\n***** Building Kibana network plugin...\n"
    git clone https://github.com/dlumbrer/kbn_network.git $SCRIPT_DIR/network_vis
    sed -i "s/5\.3\.0/5\.4\.0/g" $SCRIPT_DIR/network_vis/package.json
    cd $SCRIPT_DIR/network_vis
    npm install
    cd $SCRIPT_DIR
fi

printf "\n***** Creating network...\n"
docker network create -d bridge bridgenet

printf "\n***** Running containers...\n"
docker run --name rabmq --hostname rabmq --net bridgenet -d -p 4369:4369 -p 5671:5671 -p 5672:5672 -p 15671:15671 -p 15672:15672 -p 25672:25672 -e 'RABBITMQ_DEFAULT_VHOST=dlt' rabbitmq:management
docker run --name elast --hostname elast --net bridgenet -d -p 9200:9200 -p 9300:9300 -e 'ES_JAVA_OPTS=-Xms512m -Xmx512m' -e 'http.host=0.0.0.0' -e 'transport.host=127.0.0.1' docker.elastic.co/elasticsearch/elasticsearch:5.4.0
docker container create --name kbana --hostname kbana --net bridgenet -p 5601:5601 -e 'ELASTICSEARCH_URL=http://elast:9200' docker.elastic.co/kibana/kibana:5.4.0
docker cp $SCRIPT_DIR/network_vis kbana:/usr/share/kibana/plugins
docker container start kbana
docker run --net bridgenet --rm -e 'RABBIT_HOST=rabmq' -e 'RABBIT_VHOST=dlt' activatedgeek/rabbitmqadmin list nodes > /dev/null 2>&1
while [ $? -ne 0 ]; do
    sleep 30
    docker run --net bridgenet --rm -e 'RABBIT_HOST=rabmq' -e 'RABBIT_VHOST=dlt' activatedgeek/rabbitmqadmin list nodes > /dev/null 2>&1
done
docker run --net bridgenet --rm -e 'RABBIT_HOST=rabmq' -e 'RABBIT_VHOST=dlt' activatedgeek/rabbitmqadmin declare exchange --vhost='dlt' name='x.lt' type='topic'
docker run --net bridgenet --rm -e 'RABBIT_HOST=rabmq' -e 'RABBIT_VHOST=dlt' activatedgeek/rabbitmqadmin declare queue --vhost='dlt' name='q.logs' durable='true'
docker run --net bridgenet --rm -e 'RABBIT_HOST=rabmq' -e 'RABBIT_VHOST=dlt' activatedgeek/rabbitmqadmin declare queue --vhost='dlt' name='q.traces' durable='true'
docker run --net bridgenet --rm -e 'RABBIT_HOST=rabmq' -e 'RABBIT_VHOST=dlt' activatedgeek/rabbitmqadmin declare binding --vhost='dlt' source='x.lt' destination_type='queue' destination='q.logs' routing_key='logs'
docker run --net bridgenet --rm -e 'RABBIT_HOST=rabmq' -e 'RABBIT_VHOST=dlt' activatedgeek/rabbitmqadmin declare binding --vhost='dlt' source='x.lt' destination_type='queue' destination='q.traces' routing_key='traces'
docker run --name ls2rmq --hostname ls2rmq --net bridgenet -d -p 1501:1501/udp -p 1502:1502/udp -v $SCRIPT_DIR/logstash-pipelines/udp-to-rmq.conf:/usr/share/logstash/pipeline/udp-to-rmq.conf -e 'XPACK_MONITORING_ENABLED=false' docker.elastic.co/logstash/logstash:5.4.0
docker run --name ls2ela --hostname ls2ela --net bridgenet -d -v $SCRIPT_DIR/logstash-pipelines/rmq-to-elastic.conf:/usr/share/logstash/pipeline/rmq-to-elastic.conf -e 'XPACK_MONITORING_ENABLED=false' docker.elastic.co/logstash/logstash:5.4.0


printf "\n***** Service details"
printf "\nRabbitMQ            http://localhost:15672       user: guest     password: guest"
printf "\nElasticsearch       http://localhost:9200        user: elastic   password: changeme"
printf "\nKibana              http://localhost:5601        user: elastic   password: changeme\n\n"
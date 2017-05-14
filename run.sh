#!/bin/bash

SCRIPT_DIR=$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )

docker stop ls2ela ls2rmq kbana elast rabmq
docker rm ls2ela ls2rmq kbana elast rabmq

docker network rm bridgenet


docker network create -d bridge bridgenet

docker run --name rabmq --hostname rabmq --net bridgenet -d -p 4369:4369 -p 5671:5671 -p 5672:5672 -p 15671:15671 -p 15672:15672 -p 25672:25672 -e 'RABBITMQ_DEFAULT_VHOST=dlt' rabbitmq:management
docker run --name elast --hostname elast --net bridgenet -d -p 9200:9200 -p 9300:9300 -e 'ES_JAVA_OPTS=-Xms1g -Xmx1g' -e 'http.host=0.0.0.0' -e 'transport.host=127.0.0.1' docker.elastic.co/elasticsearch/elasticsearch:5.4.0
docker run --name kbana --hostname kbana --net bridgenet -d -p 5601:5601 -e 'ELASTICSEARCH_URL=http://elast:9200' docker.elastic.co/kibana/kibana:5.4.0

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

#docker run --name ls2rmq --hostname ls2rmq --net bridgenet --rm -i -t -p 1501:1501/udp -p 1502:1502/udp -v $SCRIPT_DIR/logstash-pipelines/udp-to-rmq.conf:/usr/share/logstash/pipeline/udp-to-rmq.conf -e 'XPACK_MONITORING_ENABLED=false' docker.elastic.co/logstash/logstash:5.4.0
docker run --name ls2rmq --hostname ls2rmq --net bridgenet -d -p 1501:1501/udp -p 1502:1502/udp -v $SCRIPT_DIR/logstash-pipelines/udp-to-rmq.conf:/usr/share/logstash/pipeline/udp-to-rmq.conf -e 'XPACK_MONITORING_ENABLED=false' docker.elastic.co/logstash/logstash:5.4.0
#docker run --name ls2ela --hostname ls2ela --net bridgenet --rm -i -t -v $SCRIPT_DIR/logstash-pipelines/rmq-to-elastic.conf:/usr/share/logstash/pipeline/rmq-to-elastic.conf -e 'XPACK_MONITORING_ENABLED=false' docker.elastic.co/logstash/logstash:5.4.0
docker run --name ls2ela --hostname ls2ela --net bridgenet -d -v $SCRIPT_DIR/logstash-pipelines/rmq-to-elastic.conf:/usr/share/logstash/pipeline/rmq-to-elastic.conf -e 'XPACK_MONITORING_ENABLED=false' docker.elastic.co/logstash/logstash:5.4.0


printf "\nElasticsearch: http://localhost:9200 (user elastic, password changeme)"
printf "\nKibana: http://localhost:5601 (user elastic, password changeme)"
printf "\nRabbitMQ: http://localhost:15672 (user guest, password guest)\n\n"
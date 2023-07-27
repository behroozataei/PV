#!/bin/sh
cp /etc/function/appsettings.json /app/appsettings.json
if [ -z "$REDIS_PASSWORD" ]
then
    until [ "$(redis-cli -h $REDIS_MASTER_HOST -p $REDIS_MASTER_PORT ping)" = "PONG" ] \
        || [ "$(redis-cli -h $REDIS_SLAVE_1_HOST -p $REDIS_SLAVE_1_PORT ping)" = "PONG" ] \
        || [ "$(redis-cli -h $REDIS_SLAVE_2_HOST -p $REDIS_SLAVE_2_PORT ping)" = "PONG" ] \
        || [ "$(redis-cli -h $REDIS_SLAVE_3_HOST -p $REDIS_SLAVE_3_PORT ping)" = "PONG" ] \
        || [ "$(redis-cli -h $REDIS_SLAVE_4_HOST -p $REDIS_SLAVE_4_PORT ping)" = "PONG" ]; do
        echo "Redis servers are not available"
        sleep 1
    done
else
    until [ "$(redis-cli -h $REDIS_MASTER_HOST -p $REDIS_MASTER_PORT -a $REDIS_PASSWORD ping)" = "PONG" ] \
        || [ "$(redis-cli -h $REDIS_SLAVE_1_HOST -p $REDIS_SLAVE_1_PORT -a $REDIS_PASSWORD ping)" = "PONG" ] \
        || [ "$(redis-cli -h $REDIS_SLAVE_2_HOST -p $REDIS_SLAVE_2_PORT -a $REDIS_PASSWORD ping)" = "PONG" ] \
        || [ "$(redis-cli -h $REDIS_SLAVE_3_HOST -p $REDIS_SLAVE_3_PORT -a $REDIS_PASSWORD ping)" = "PONG" ] \
        || [ "$(redis-cli -h $REDIS_SLAVE_4_HOST -p $REDIS_SLAVE_4_PORT -a $REDIS_PASSWORD ping)" = "PONG" ]; do
        echo "Redis servers are not available"
        sleep 1
    done
fi
echo "Run OPC"
./Irisa.OPC







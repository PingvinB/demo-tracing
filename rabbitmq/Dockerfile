FROM rabbitmq:latest

RUN rabbitmq-plugins enable --offline rabbitmq_stream && \
    rabbitmq-plugins enable --offline rabbitmq_stream_management

EXPOSE 5672 15672 5552 

CMD ["rabbitmq-server"]

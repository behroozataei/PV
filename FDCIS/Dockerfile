FROM func_runtime:latest
WORKDIR /app
COPY . . 
RUN mkdir -p /etc/function
RUN mkdir -p /app/rep
RUN chmod +x FDCIS
RUN chmod +x fdcis-entrypoint.sh 
ENTRYPOINT ["./fdcis-entrypoint.sh"]
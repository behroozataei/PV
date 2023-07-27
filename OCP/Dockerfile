FROM func_runtime:latest
WORKDIR /app
COPY . . 
RUN mkdir -p /etc/function
RUN chmod +x OCP
RUN chmod +x ocp-entrypoint.sh 
ENTRYPOINT ["./ocp-entrypoint.sh"]
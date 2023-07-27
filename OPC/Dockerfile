FROM func_runtime:latest
WORKDIR /app
COPY . . 
RUN mkdir -p /etc/function
RUN chmod +x Irisa.OPC
RUN chmod +x opc-entrypoint.sh 
ENTRYPOINT ["./opc-entrypoint.sh"]
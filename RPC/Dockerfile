FROM func_runtime:latest
WORKDIR /app
COPY . . 
RUN chmod +x RPC
RUN chmod +x rpc-entrypoint.sh 
ENTRYPOINT ["./rpc-entrypoint.sh"]
FROM func_runtime:latest
WORKDIR /app
COPY . . 
RUN mkdir -p /etc/function
RUN chmod +x SFD
RUN chmod +x sfd-entrypoint.sh 
ENTRYPOINT ["./sfd-entrypoint.sh"]
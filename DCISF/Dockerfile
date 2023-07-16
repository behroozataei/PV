FROM func_runtime:latest
WORKDIR /app
COPY . . 
RUN chmod +x DCISF
RUN chmod +x dcisf-entrypoint.sh 
ENTRYPOINT ["./dcisf-entrypoint.sh"]
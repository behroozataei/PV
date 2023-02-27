FROM func_runtime:latest
WORKDIR /app
COPY . . 
RUN chmod +x MAB
RUN chmod +x mab-entrypoint.sh 
ENTRYPOINT ["./mab-entrypoint.sh"]
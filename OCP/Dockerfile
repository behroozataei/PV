FROM dotnet_runtime:latest
WORKDIR /app
COPY . . 
RUN chmod +x OCP
RUN chmod +x ocp-entrypoint.sh 
ENTRYPOINT ["./ocp-entrypoint.sh"]
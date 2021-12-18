FROM mcr.microsoft.com/dotnet/runtime:5.0
RUN apt-get update -y\
&& apt-get install -y redis-tools
COPY . /app/ 
RUN chmod +x app/Irisa.OPC 
RUN chmod +x app/opc-entrypoint.sh 
WORKDIR /app
ENTRYPOINT ["./opc-entrypoint.sh"]


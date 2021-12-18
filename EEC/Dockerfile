FROM mcr.microsoft.com/dotnet/runtime:5.0
RUN apt-get update -y\
&& apt-get install -y redis-tools
COPY . /app/ 
RUN chmod +x app/EEC
RUN chmod +x app/eec-entrypoint.sh 
WORKDIR /app
ENTRYPOINT ["./eec-entrypoint.sh"]
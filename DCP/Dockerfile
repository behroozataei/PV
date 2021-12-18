FROM mcr.microsoft.com/dotnet/runtime:5.0
RUN apt-get update -y\
&& apt-get install -y redis-tools
COPY . /app/ 
RUN chmod +x app/DCP
RUN chmod +x app/dcp-entrypoint.sh 
WORKDIR /app
ENTRYPOINT ["./dcp-entrypoint.sh"]
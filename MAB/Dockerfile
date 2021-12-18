FROM mcr.microsoft.com/dotnet/runtime:5.0
RUN apt-get update -y\
&& apt-get install -y redis-tools
COPY . /app/ 
RUN chmod +x app/MAB 
RUN chmod +x app/mab-entrypoint.sh 
WORKDIR /app
ENTRYPOINT ["./mab-entrypoint.sh"]

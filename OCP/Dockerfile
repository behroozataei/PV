FROM mcr.microsoft.com/dotnet/runtime:5.0
RUN apt-get update -y\
&& apt-get install -y redis-tools
COPY . /app/ 
RUN chmod +x app/OCP 
RUN chmod +x app/ocp-entrypoint.sh 
WORKDIR /app
ENTRYPOINT ["./ocp-entrypoint.sh"]
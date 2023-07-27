#FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
#RUN apt-get update -y \
#    && apt-get install -y  redis-tools

#WORKDIR /app
#COPY . . 

#FROM mcr.microsoft.com/dotnet/sdk:3.1 AS buildTools
#LABEL maintainer S.Khanbazi
#RUN dotnet tool install --tool-path /tools dotnet-trace
#RUN dotnet tool install --tool-path /tools dotnet-counters
#RUN dotnet tool install --tool-path /tools dotnet-dump
#RUN dotnet tool install --tool-path /tools dotnet-gcdump

#FROM base as publish
#WORKDIR /tools
#COPY --from=buildTools /tools .
#WORKDIR /app
FROM func_runtime:latest
WORKDIR /app
COPY . . 
RUN mkdir -p /etc/function
RUN chmod +x DCP
RUN chmod +x dcp-entrypoint.sh 
ENTRYPOINT ["./dcp-entrypoint.sh"]
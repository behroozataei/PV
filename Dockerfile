FROM mcr.microsoft.com/dotnet/sdk:3.1
LABEL maintainer S.Khanbazi
RUN dotnet tool install --tool-path /tools dotnet-trace
RUN dotnet tool install --tool-path /tools dotnet-counters
RUN dotnet tool install --tool-path /tools dotnet-dump
RUN dotnet tool install --tool-path /tools dotnet-gcdump
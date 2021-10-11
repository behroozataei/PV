FROM mcr.microsoft.com/dotnet/runtime:5.0
RUN apt-get update -y
WORKDIR /app
COPY . . 
RUN chmod +x LSP
CMD ./LSP
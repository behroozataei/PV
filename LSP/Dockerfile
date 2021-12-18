FROM mcr.microsoft.com/dotnet/runtime:5.0
RUN apt-get update -y\
&& apt-get install -y redis-tools
COPY . /app/ 
RUN chmod +x app/LSP
RUN chmod +x app/lsp-entrypoint.sh 
WORKDIR /app
ENTRYPOINT ["./lsp-entrypoint.sh"]
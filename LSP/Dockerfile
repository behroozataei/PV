FROM dotnet_runtime:latest
WORKDIR /app
COPY . . 
RUN chmod +x LSP
RUN chmod +x lsp-entrypoint.sh 
ENTRYPOINT ["./lsp-entrypoint.sh"]
FROM func_runtime:latest
WORKDIR /app
COPY . . 
RUN mkdir -p /etc/function
RUN chmod +x LSP
RUN chmod +x lsp-entrypoint.sh 
ENTRYPOINT ["./lsp-entrypoint.sh"]
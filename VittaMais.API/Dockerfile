# Estágio 1: Compilação da Aplicação (Build)
# Imagem do SDK do .NET 8, que tem todas as ferramentas para compilar o projeto.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copia o arquivo .csproj e restaura as dependências primeiro.
# Aproveita o cache de camadas do Docker. Se os pacotes não mudarem, o Docker reutiliza essa camada, acelerando builds futuros.
COPY ["VittaMais.API/VittaMais.API.csproj", "VittaMais.API/"]
RUN dotnet restore "VittaMais.API/VittaMais.API.csproj"

# Copia o resto do código-fonte do projeto.
COPY ["VittaMais.API/", "VittaMais.API/"]
WORKDIR "/source/VittaMais.API"
# Publica a aplicação em modo Release, otimizada para produção.
RUN dotnet publish "VittaMais.API.csproj" -c Release -o /app/publish --no-restore

# Estágio 2: Imagem Final (Runtime)
# Imagem do ASP.NET Runtime, muito menor e mais segura, contém apenas o necessário para EXECUTAR a aplicação.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
# Copia apenas os arquivos publicados do estágio de build.
COPY --from=build /app/publish .
# Expõe a porta 8080 e configura a aplicação para escutar nela.
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
# Define o comando para iniciar a aplicação quando o contêiner for executado.
ENTRYPOINT ["dotnet", "VittaMais.API.dll"]
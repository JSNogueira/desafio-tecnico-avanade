# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia o arquivo de projeto da API de Vendas
COPY Vendas/Vendas.csproj ./Vendas/
# Copia também o projeto compartilhado
COPY MensagensCompartilhadas/MensagensCompartilhadas.csproj ./MensagensCompartilhadas/

# Restaura dependências
RUN dotnet restore Vendas/Vendas.csproj

# Copia todo o código-fonte das APIs e bibliotecas
COPY . .

# Publica a API de Vendas
RUN dotnet publish Vendas/Vendas.csproj -c Release -o /app

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Vendas.dll"]
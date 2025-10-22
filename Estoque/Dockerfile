FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY Estoque/Estoque.csproj ./Estoque/
COPY MensagensCompartilhadas/MensagensCompartilhadas.csproj ./MensagensCompartilhadas/
RUN dotnet restore Estoque/Estoque.csproj

COPY . .
RUN dotnet publish Estoque/Estoque.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Estoque.dll"]
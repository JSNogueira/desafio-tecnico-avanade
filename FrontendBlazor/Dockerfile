FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY FrontendBlazor.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

RUN rm -rf ./*

COPY --from=build /app/publish/wwwroot .

COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
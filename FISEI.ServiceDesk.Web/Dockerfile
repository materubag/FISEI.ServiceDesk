# Etapa 1: construir CSS con Tailwind
FROM node:20-alpine AS tailwind
WORKDIR /src
# Copiamos todo el repo para que tailwind pueda escanear los .razor
COPY . .
# Instalar dependencias (usa package.json dentro de FISEI.ServiceDesk.Web)
WORKDIR /src/FISEI.ServiceDesk.Web
RUN npm install && \
    npx tailwindcss -i ./wwwroot/app.css -o ./wwwroot/app.bundle.css --minify

# Etapa 2: publicar el proyecto .NET
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
# Copiar el CSS generado por Tailwind desde la etapa node
COPY --from=tailwind /src/FISEI.ServiceDesk.Web/wwwroot/app.bundle.css FISEI.ServiceDesk.Web/wwwroot/app.bundle.css
RUN dotnet restore ./FISEI.ServiceDesk.sln
RUN dotnet publish ./FISEI.ServiceDesk.Web/FISEI.ServiceDesk.Web.csproj -c Release -o /app/publish

# Etapa 3: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
# Puerto interno (Blazor Server normalmente en 8080 si así lo expones)
EXPOSE 8080
ENTRYPOINT ["dotnet","FISEI.ServiceDesk.Web.dll"]
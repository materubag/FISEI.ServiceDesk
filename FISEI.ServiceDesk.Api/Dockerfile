FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copia toda la solución (build context es la raíz del repo)
COPY . .

# Restaurar y publicar el proyecto API
RUN dotnet restore ./FISEI.ServiceDesk.sln
RUN dotnet publish ./FISEI.ServiceDesk.Api/FISEI.ServiceDesk.Api.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "FISEI.ServiceDesk.Api.dll"]
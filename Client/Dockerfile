# 🔹 Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar tudo do projeto
COPY . .

# Restaurar dependências
RUN dotnet restore

# Publicar aplicação
RUN dotnet publish -c Release -o /app

# 🔹 Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./

ENTRYPOINT ["dotnet", "Client.dll"]

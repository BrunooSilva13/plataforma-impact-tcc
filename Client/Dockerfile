# 🔹 Imagem base do SDK .NET para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos do projeto
COPY ./*.csproj ./
COPY . .

# Restaurar dependências e publicar
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# 🔹 Imagem runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./

ENTRYPOINT ["dotnet", "Client.dll"]

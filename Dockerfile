# Sử dụng .NET 8 runtime làm base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://0.0.0.0:8080

# Sử dụng .NET 8 SDK để build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["AdmissionInfoSystem.csproj", "./"]
RUN dotnet restore "AdmissionInfoSystem.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "AdmissionInfoSystem.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AdmissionInfoSystem.csproj" -c Release -o /app/publish --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AdmissionInfoSystem.dll"] 
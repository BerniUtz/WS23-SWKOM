FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["SWKOM-paperless.OCRWorker.csproj", "SWKOM-paperless.OCRWorker/"]

# adding other projectdependencies
#COPY ["../SWKOM-paperless.BusinessLogic/SWKOM-paperless.BusinessLogic.csproj", "SWKOM-paperless.BusinessLogic/"]
#COPY ["../SWKOM-paperless.BusinessLogic.EntityValidators/SWKOM-paperless.BusinessLogic.EntityValidators.csproj", "SWKOM-paperless.BusinessLogic.EntityValidators/"]
#COPY ["../SWKOM-paperless.BusinessLogic.Entities/SWKOM-paperless.BusinessLogic.Entities.csproj", "SWKOM-paperless.BusinessLogic.Entities/"]
#COPY ["../SWKOM-paperless.BusinessLogic.Interfaces/SWKOM-paperless.BusinessLogic.Interfaces.csproj", "SWKOM-paperless.BusinessLogic.Interfaces/"]
#COPY ["../SWKOM-paperless.ServiceAgents/SWKOM-paperless.ServiceAgents.csproj", "SWKOM-paperless.ServiceAgents/"]
COPY ["../SWKOM-paperless.ServiceAgents.Interfaces/SWKOM-paperless.ServiceAgents.Interfaces.csproj", "SWKOM-paperless.ServiceAgents.Interfaces/"]

RUN dotnet restore "SWKOM-paperless.OCRWorker/SWKOM-paperless.OCRWorker.csproj"

COPY . .
WORKDIR "/src/SWKOM-paperless.OCRWorker"
RUN dotnet build "SWKOM-paperless.OCRWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SWKOM-paperless.OCRWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SWKOM-paperless.OCRWorker.dll"]
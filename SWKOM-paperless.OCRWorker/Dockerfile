# Use the official Microsoft .NET Core SDK image as the base image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build

# Set the working directory inside the container
WORKDIR /app

# Copy the main solution file and the specific project file you need to build
COPY *.sln ./
COPY SWKOM-paperless.OCRWorker/SWKOM-paperless.OCRWorker.csproj SWKOM-paperless.OCRWorker/

# Restore the specific project you're building
RUN dotnet restore SWKOM-paperless.OCRWorker/SWKOM-paperless.OCRWorker.csproj

# Copy everything else and build the project
# Note: You need to ensure that you're copying all the necessary files for your project to build
COPY . ./
RUN dotnet publish SWKOM-paperless.OCRWorker/SWKOM-paperless.OCRWorker.csproj -c Release -o out

# Generate runtime image (asp.net core runtime)
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app
COPY --from=build /app/out .

# Set the command to run your application
ENTRYPOINT ["dotnet", "SWKOM-paperless.OCRWorker.dll"]

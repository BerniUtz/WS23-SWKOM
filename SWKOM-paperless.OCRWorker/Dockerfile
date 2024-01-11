FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
ARG BUILD_CONFIG=Release
WORKDIR /App

# Copy the main solution file and the specific project file you need to build
COPY *.sln ./
COPY SWKOM-paperless.OCRWorker/SWKOM-paperless.OCRWorker.csproj SWKOM-paperless.OCRWorker/

# Restore the specific project you're building
RUN dotnet restore SWKOM-paperless.OCRWorker/SWKOM-paperless.OCRWorker.csproj

# Copy everything else and build the project
# Note: You need to ensure that you're copying all the necessary files for your project to build
COPY . ./
RUN dotnet publish SWKOM-paperless.OCRWorker/SWKOM-paperless.OCRWorker.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /App
COPY --from=build-env /App/out .

# for remote debugger
#RUN apt update && apt install unzip && url -sSL https://aka.ms/getvsdbgsh | /bin/sh /dev/stdin -v latest -l /vsdbg

# some basic dependencies
RUN apt-get update -y && apt-get install -y libc6-dev libleptonica-dev

# Copied & adapted some part from https://github.com/AlexZeitler/tesseract-ocr-dotnet6-linux-docker/blob/main/TeasseractOCROnLInux/Dockerfile

# needed to get tesseract 5 
RUN apt-get update -y && apt-get install -y wget gnupg2 apt-transport-https software-properties-common
RUN tee /etc/apt/sources.list.d/notesalexp.list<<EOF
RUN echo "deb https://notesalexp.org/tesseract-ocr5/$(lsb_release -cs)/ $(lsb_release -cs) main" | tee /etc/apt/sources.list.d/notesalexp.list > /dev/null
RUN wget -O - https://notesalexp.org/debian/alexp_key.asc | apt-key add - 

# tesseract
RUN apt-get update -y && apt-get install -y tesseract-ocr

# ghostscript
RUN apt-get update -y && apt-get install -y ghostscript libgs-dev

# get architecture
ARG TARGETPLATFORM
RUN if [ "$TARGETPLATFORM" = "linux/amd64" ]; then \
        export ARCHITECTURE="x86_64"; \
        export CPU_BIT="64"; \
    elif [ "$TARGETPLATFORM" = "linux/arm/v7" ]; then \
        export ARCHITECTURE="arm"; \
        export CPU_BIT="32"; \
    elif [ "$TARGETPLATFORM" = "linux/arm64" ]; then \
        ARCHITECTURE="aarch64"; \
    else \
        export ARCHITECTURE=x86; \
        export CPU_BIT=32; \
    fi

# needed for macos & linux arm64 (because .NET is doing Microsoft's Only-Windows-Policy again)
RUN if [ "$CPU_BIT" != "64" ]; then \
        if [ "$ARCHITECTURE" != "x86_64" ]; then \
            ln -s /usr/lib/aarch64-linux-gnu /usr/lib/x86_64-linux-gnu; \
        fi \
    fi

RUN ln -s /usr/lib/x86_64-linux-gnu/liblept.so.5 /App/x64/libleptonica-1.82.0.so
RUN ln -s /usr/lib/x86_64-linux-gnu/libtesseract.so.5 /App/x64/libtesseract50.so

# some libs are needed
RUN ln -s /usr/lib/x86_64-linux-gnu/* /App/x64/

ENTRYPOINT ["dotnet", "SWKOM-paperless.OCRWorker.dll"]
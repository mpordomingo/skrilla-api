FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build-env
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        curl \
        wget \
        gpg \
        apt-transport-https \
    && rm -rf /var/lib/apt/lists/*

RUN apt-get update
RUN wget https://packages.microsoft.com/config/debian/10/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
RUN dpkg -i packages-microsoft-prod.deb
RUN  apt-get update \
  &&  apt-get install -y apt-transport-https && \
   apt-get update && \
   apt-get install -y dotnet-sdk-2.1


RUN dotnet --info
RUN apt-get install ca-certificates

# Copy csproj and restore as distinct layers
COPY skrilla-api/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY skrilla-api/. ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:3.1
#FROM microsoft/dotnet:2.0-sdk 
WORKDIR /app
COPY --from=build-env /app/out .
CMD dotnet SkrillaApi.dll

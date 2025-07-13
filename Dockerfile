# AMD64 needs to be forced because compilation on ARM QEMU is fucked up.

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH=amd64
ARG TARGETOS=linux

WORKDIR /src

RUN apt update
RUN apt install -y clang llvm

# https://github.com/dotnet/sdk/issues/28971#issuecomment-1308881150
#RUN arch=$TARGETARCH \
#    && if [ "$arch" = "amd64" ]; then arch="x64"; fi \
#    && echo $TARGETOS-$arch > /tmp/rid 
    
COPY . .
RUN dotnet restore "MikrotikExporter/MikrotikExporter.csproj"

RUN dotnet publish "MikrotikExporter/MikrotikExporter.csproj" \
    -c $BUILD_CONFIGURATION \
    --self-contained \
#    -r $(cat /tmp/rid) \
    -o /app/build

FROM debian:12-slim AS runtime
WORKDIR /app

LABEL org.opencontainers.image.source=https://github.com/ScuroGuardiano/MikrotikExporter
LABEL org.opencontainers.image.description="Mikrotik Promehetus metrics expoter"
LABEL org.opencontainers.image.licenses=AGPLv3

ENV ASPNETCORE_URLS=http://0.0.0.0:5000
EXPOSE 5000

COPY --from=build /app/build ./

ENTRYPOINT ["./MikrotikExporter"] 
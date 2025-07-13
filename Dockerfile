# AMD64 needs to be forced because compilation on ARM QEMU is fucked up.

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine-amd64 AS prepare

ARG BUILD_CONFIGURATION=Release


WORKDIR /src


COPY . .
RUN dotnet restore "MikrotikExporter/MikrotikExporter.csproj"

FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
RUN apk add --no-cache clang zlib-dev

ARG TARGETARCH
ARG TARGETOS

WORKDIR /src

# https://github.com/dotnet/sdk/issues/28971#issuecomment-1308881150

RUN arch=$TARGETARCH \
    && if [ "$arch" = "amd64" ]; then arch="x64"; fi \
    && echo $TARGETOS-musl-$arch > /tmp/rid 

COPY --from=prepare /src ./

RUN dotnet publish "MikrotikExporter/MikrotikExporter.csproj" \
    -c $BUILD_CONFIGURATION \
    --self-contained \
    -r $(cat /tmp/rid) \
    /p:PublishAot=true \
    -o /app/build

FROM alpine:3.22 AS runtime
WORKDIR /app

LABEL org.opencontainers.image.source=https://github.com/ScuroGuardiano/MikrotikExporter
LABEL org.opencontainers.image.description="Mikrotik Promehetus metrics expoter"
LABEL org.opencontainers.image.licenses=AGPLv3

ENV ASPNETCORE_URLS=http://0.0.0.0:5000
EXPOSE 5000
RUN apk add --no-cache libstdc++ icu-libs

COPY --from=build /app/build ./

ENTRYPOINT ["./MikrotikExporter"] 
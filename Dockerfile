FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Instalacja zależności dla Native AOT w Alpine
RUN apk add --no-cache clang zlib-dev

# Kopiowanie i restore projektu
COPY . .
RUN dotnet restore "MikrotikExporter/MikrotikExporter.csproj"

# Publikacja jako Native AOT dla linux-musl-x64
RUN dotnet publish "MikrotikExporter/MikrotikExporter.csproj" \
    -c $BUILD_CONFIGURATION \
    --self-contained \
    /p:PublishAot=true \
    -o /app/build

FROM alpine:3.22 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:5000
EXPOSE 5000
RUN apk add --no-cache libstdc++ icu-libs

COPY --from=build /app/build ./

ENTRYPOINT ["./MikrotikExporter"] 
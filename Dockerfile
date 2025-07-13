# This is fucking wordaround, so first of all, if we want to compile our cute
# little program with NativeAOT to ARMv7, we can't use ARMv7 image on QEMU.
# It will fail with some hardware error XD
# So we can cross-compile. But...
FROM --platform=linux/amd64 mcr.microsoft.com/dotnet/sdk:9.0 AS build

RUN apt update
RUN apt install -y file clang llvm zlib1g-dev gcc-aarch64-linux-gnu binutils-aarch64-linux-gnu gcc-arm-linux-gnueabihf binutils-arm-linux-gnueabihf

ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH=amd64
ARG TARGETOS=linux

WORKDIR /src

# https://github.com/dotnet/sdk/issues/28971#issuecomment-1308881150
RUN arch=$TARGETARCH \
    && if [ "$arch" = "amd64" ]; then arch="x64"; fi \
    && echo $TARGETOS-$arch > /tmp/rid 
    
COPY . .

# ...cross compilation for ARMv7 calles clang with --target=armv7-linux-gnueabihf
# Which is not the right target, it should be 'arm-linux-gnueabihf'
# Since this god damn NativeAOT compilation is almost undocumented I have
# no god damn idea if it's even possible to change it from dotnet publish.
# So I created wrapper around clang that changes this setting
# With that it should finally work ;-;
RUN cp /usr/bin/clang /usr/bin/clang-orig
RUN rm /usr/bin/clang
RUN cp clang-wrapper.sh /usr/bin/clang
RUN chmod u+x /usr/bin/clang

RUN dotnet publish "MikrotikExporter/MikrotikExporter.csproj" \
    -c $BUILD_CONFIGURATION \
    --self-contained \
    -r $(cat /tmp/rid) \
    -o /app/build

RUN file /app/build/MikrotikExporter

FROM debian:12-slim AS runtime
RUN apt update
RUN apt install -y libicu72

WORKDIR /app

LABEL org.opencontainers.image.source=https://github.com/ScuroGuardiano/MikrotikExporter
LABEL org.opencontainers.image.description="Mikrotik Promehetus metrics expoter"
LABEL org.opencontainers.image.licenses=AGPLv3

ENV ASPNETCORE_URLS=http://0.0.0.0:5000
EXPOSE 5000

COPY --from=build /app/build ./


ENTRYPOINT ["./MikrotikExporter"] 
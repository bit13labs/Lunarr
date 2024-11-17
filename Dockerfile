# syntax=docker/dockerfile:1

FROM ghcr.io/linuxserver/baseimage-alpine:3.20

# set version label
ARG BUILD_DATE
ARG VERSION
ARG LUNARR_RELEASE
LABEL build_version="${VERSION} Build-date:- ${BUILD_DATE}"
LABEL maintainer=""

# environment settings
ARG LUNARR_BRANCH="lunarr"
ENV XDG_CONFIG_HOME="/config/xdg" \
  COMPlus_EnableDiagnostics=0 \
  TMPDIR=/run/lunarr-temp

RUN \
  echo "**** install packages ****" && \
  apk add -U --upgrade --no-cache \
  icu-libs \
  sqlite-libs \
  xmlstarlet && \
  echo "**** install lunarr ****" && \
  mkdir -p /app/radarr/bin && \
  if [ -z ${LUNARR_RELEASE+x} ]; then \
  LUNARR_RELEASE="1.0.0"; \
  fi && \
  curl -o \
  /tmp/lunarr.tar.gz -L \
  "https://github.com/bit13labs/Lunarr/raw/refs/heads/lunarr/lunarr-v1.0.0-linux-x64.tar.gz" && \
  tar xzf \
  /tmp/radarr.tar.gz -C \
  /app/radarr/bin --strip-components=1 && \
  echo -e "UpdateMethod=docker\nBranch=${LUNARR_BRANCH}\nPackageVersion=${VERSION}\nPackageAuthor=Anonymous" > /app/lunarr/package_info && \
  printf "version: ${VERSION}\nBuild-date: ${BUILD_DATE}" > /build_version && \
  echo "**** cleanup ****" && \
  rm -rf \
  /app/lunarr/bin/Lunarr.Update \
  /tmp/*

# copy local files
COPY files/root/ /

# ports and volumes
EXPOSE 6969

VOLUME /config
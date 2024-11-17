# syntax=docker/dockerfile:1

FROM ghcr.io/linuxserver/baseimage-alpine:3.20

# set version label
ARG BUILD_DATE
ARG VERSION
ARG LUNARR_RELEASE
LABEL build_version="Linuxserver.io version:- ${VERSION} Build-date:- ${BUILD_DATE}"
LABEL maintainer="Roxedus,thespad"

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
  if [ -z ${RADARR_RELEASE+x} ]; then \
  RADARR_RELEASE=$(curl -sL "https://whisparr.servarr.com/v1/update/${LUNARR_BRANCH}/changes?runtime=netcore&os=linuxmusl" \
  | jq -r '.[0].version'); \
  fi && \
  curl -o \
  /tmp/radarr.tar.gz -L \
  "https://radarr.servarr.com/v1/update/${LUNARR_BRANCH}/updatefile?version=${LUNARR_RELEASE}&os=linuxmusl&runtime=netcore&arch=x64" && \
  tar xzf \
  /tmp/radarr.tar.gz -C \
  /app/radarr/bin --strip-components=1 && \
  echo -e "UpdateMethod=docker\nBranch=${LUNARR_BRANCH}\nPackageVersion=${VERSION}\nPackageAuthor=[linuxserver.io](https://linuxserver.io)" > /app/lunarr/package_info && \
  printf "Linuxserver.io version: ${VERSION}\nBuild-date: ${BUILD_DATE}" > /build_version && \
  echo "**** cleanup ****" && \
  rm -rf \
  /app/radarr/bin/Radarr.Update \
  /tmp/*

# copy local files
COPY root/ /

# ports and volumes
EXPOSE 7878

VOLUME /config
FROM mcr.microsoft.com/dotnet/core/sdk:3.1

LABEL maintainer="svanhildur19@ru.is"

ENV ASPNETCORE_URLS https://*:80

WORKDIR /app/Cryptocop.Software.API
COPY . /app

RUN dotnet restore
RUN dotnet build

ENTRYPOINT [ "dotnet", "run"]
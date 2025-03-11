FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /chatapp/

# restore projects
COPY *.sln .
COPY ChatApplication.Web/*.csproj ./ChatApplication.Web/
RUN dotnet restore

# generate the native dll that we will run
COPY ChatApplication.Web/. ./ChatApplication.Web/

WORKDIR /chatapp/ChatApplication.Web/
RUN dotnet publish -c release -o /bin/release/ --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /chatapp/
COPY --from=build /chatapp/ChatApplication.Web/bin/release ./ChatApplication.Web/

COPY ./entrypoint ./
RUN chmod +x ./entrypoint

ENTRYPOINT ["./entrypoint"]
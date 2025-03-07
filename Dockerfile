# run Producer app first

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /chatapp/

ENV ASPNETCORE_URLS=http://+:5000

EXPOSE 5000


# restore projects
COPY *.sln .
COPY ChatApplication.Producer/*.csproj ./ChatApplication.Producer/
COPY ChatApplication.Shared/*.csproj ./ChatApplication.Shared/
COPY ChatApplication.Consumer/*.csproj ./ChatApplication.Consumer/
RUN dotnet restore

# generate the native dll that we will run
COPY ChatApplication.Producer/. ./ChatApplication.Producer/
COPY ChatApplication.Shared/. ./ChatApplication.Shared/
COPY ChatApplication.Consumer/. ./ChatApplication.Consumer/

WORKDIR /chatapp/ChatApplication.Producer/
RUN dotnet publish -c release -o /bin/release/ --no-restore
WORKDIR /chatapp/ChatApplication.Shared
RUN dotnet publish -c release -o /bin/release/ --no-restore
WORKDIR /chatapp/ChatApplication.Consumer
RUN dotnet publish -c release -o /bin/release/ --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /chatapp/
COPY --from=build /chatapp/ChatApplication.Producer/bin/release ./ChatApplication.Producer/
COPY --from=build /chatapp/ChatApplication.Shared/bin/release ./ChatApplication.Shared/
COPY --from=build /chatapp/ChatApplication.Consumer/bin/release ./ChatApplication.Consumer/

COPY entrypoint ./
RUN chmod +x ./entrypoint

ENTRYPOINT ["./entrypoint"]

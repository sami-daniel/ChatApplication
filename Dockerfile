FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /chatapp/

# copy sln file and .csproj files to the container and restore the project dependecies
COPY *.sln .
COPY ChatApplication.Web/*.csproj ./ChatApplication.Web/
RUN dotnet restore

# copy the wwwroot (static assests) before publish, cuz it will be embbedded on the final DLL if not moved.
COPY ChatApplication.Web/wwwroot/ ./wwwroot/

# copy the rest of the files (source)
COPY ChatApplication.Web/. ./ChatApplication.Web/

# now publish (compile and publish to the /bin/release folder)
WORKDIR /chatapp/ChatApplication.Web/
RUN dotnet publish -c release -o /bin/release/ --no-restore

# now we prepare the env for run the project
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /chatapp/

# copy the dlls to ChatApplication.Web
COPY --from=build /chatapp/ChatApplication.Web/bin/release ./ChatApplication.Web/

# copy explicit the wwwroot to avoid the missing
COPY --from=build /chatapp/wwwroot ./wwwroot/

# copy entrypoint and make it executable
COPY ./entrypoint ./
RUN chmod +x ./entrypoint

# run it
ENTRYPOINT ["./entrypoint"]
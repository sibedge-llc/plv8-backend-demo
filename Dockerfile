FROM mcr.microsoft.com/dotnet/sdk:5.0.301-alpine3.13-amd64 AS installer
WORKDIR /usr
COPY . .
RUN dotnet restore
WORKDIR /usr/${MODULE_NAME}
RUN dotnet build -o ./build \
  && dotnet publish -o ./build --no-restore

FROM  mcr.microsoft.com/dotnet/sdk:5.0.301-alpine3.13-amd64
WORKDIR /usr/${MODULE_NAME}/
COPY --from=installer /usr/${MODULE_NAME}/ .
ENTRYPOINT dotnet build/${DLL_NAME} migrateforce start
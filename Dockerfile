FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine3.18 AS installer
WORKDIR /usr
COPY . .
RUN dotnet restore
WORKDIR /usr/${MODULE_NAME}
RUN dotnet build -c release -o ./build --no-restore \
  && dotnet publish -c release -o ./publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine3.18
WORKDIR /usr/${MODULE_NAME}/
COPY --from=installer /usr/${MODULE_NAME}/publish .
ENTRYPOINT dotnet ${MODULE_NAME}.dll

FROM microsoft/dotnet:sdk AS build-env
WORKDIR /src

COPY /src .

WORKDIR /src/PaymentsSystemExample.Api/
RUN dotnet restore PaymentsSystemExample.Api.csproj
RUN dotnet build --no-restore -o /app

FROM build-env AS publish
RUN dotnet publish --no-restore -c Debug -o /app

FROM microsoft/dotnet:2.1-aspnetcore-runtime
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "PaymentsSystemExample.Api.dll"]

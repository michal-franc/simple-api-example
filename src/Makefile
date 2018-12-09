.PHONY: unit-test, auto-test, integration-test, all-tests

unit-test:
	dotnet test PaymentsSystemExample.UnitTests/PaymentsSystemExample.UnitTests.csproj

integration-test:
	dotnet test PaymentsSystemExample.IntegrationTests/PaymentsSystemExample.IntegrationTests.csproj

auto-test:
	rg --files -tcsharp | entr make unit-test

all-tests: unit-test, integration-test

build:
	dotnet build PaymentsSystemExample.Api/PaymentsSystemExample.Api.csproj
	dotnet build PaymentsSystemExample.Domain/PaymentsSystemExample.Domain.csproj
	dotnet build PaymentsSystemExample.UnitTests/PaymentsSystemExample.UnitTests.csproj
	dotnet build PaymentsSystemExample.IntegrationTests/PaymentsSystemExample.IntegrationTests.csproj

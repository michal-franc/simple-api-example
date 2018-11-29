.PHONY: unit-test, auto-test

unit-test:
	dotnet test PaymentsSystemExample.UnitTests/PaymentsSystemExample.UnitTests.csproj
	dotnet test PaymentsSystemExample.IntegrationTests/PaymentsSystemExample.IntegrationTests.csproj

auto-test:
	rg --files -tcsharp | entr make unit-test

build:
	dotnet build PaymentsSystemExample.UnitTests/PaymentsSystemExample.UnitTests.csproj
	dotnet build PaymentsSystemExample.Api/payment-system-example.csproj

.PHONY: unit-test, auto-test

unit-test:
	dotnet test PaymentsSystemExample.UnitTests/PaymentsSystemExample.UnitTests.csproj

auto-test:
	rg --files -tcsharp | entr make unit-test
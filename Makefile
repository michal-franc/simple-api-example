.PHONY: unit-test, auto-test, integration-test, dynamodb, all-tests, put-multi, put-single

start: dynamodb
	dotnet run --project src/PaymentsSystemExample.Api/PaymentsSystemExample.Api.csproj

stop:	
	cd test-infra && docker-compose down

build:
	dotnet build src/PaymentsSystemExample.Api/PaymentsSystemExample.Api.csproj
	dotnet build src/PaymentsSystemExample.Domain/PaymentsSystemExample.Domain.csproj
	dotnet build src/PaymentsSystemExample.UnitTests/PaymentsSystemExample.UnitTests.csproj
	dotnet build src/PaymentsSystemExample.IntegrationTests/PaymentsSystemExample.IntegrationTests.csproj

dynamodb:
	cd test-infra && docker-compose up -d
	sleep 5
	- cd test-infra && ./create-test-table.sh
	sleep 2

all-tests: unit-test dynamodb integration-test stop

put-multi:
	curl -X PUT localhost:5000/api/v1/payment -H "X-CultureCode:en-GB" -H "Content-Type:application/json" -d @test-data/multiple_payments_payload_test.json

put-single:
	curl -X PUT localhost:5000/api/v1/payment -H "X-CultureCode:en-GB" -H "Content-Type:application/json" -d @test-data/one_payment_payload_test.json

unit-test:
	- dotnet test src/PaymentsSystemExample.UnitTests/PaymentsSystemExample.UnitTests.csproj

integration-test:
	- dotnet test src/PaymentsSystemExample.IntegrationTests/PaymentsSystemExample.IntegrationTests.csproj

auto-test:
	rg --files -tcsharp | entr make unit-test

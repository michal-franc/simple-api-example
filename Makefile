.PHONY: unit-test, auto-test, integration-test, all-tests, put-multi, put-single

run:
	cd test-infra && docker-compose up -d
	sleep 5
	cd test-infra && ./create-test-table.sh
	sleep 2
	dotnet run --project src/PaymentsSystemExample.Api/PaymentsSystemExample.Api.csproj
	touch run

stop:	
	cd test-infra && docker-compose down
	rm run

build:
	dotnet build src/PaymentsSystemExample.Api/PaymentsSystemExample.Api.csproj
	dotnet build src/PaymentsSystemExample.Domain/PaymentsSystemExample.Domain.csproj
	dotnet build src/PaymentsSystemExample.UnitTests/PaymentsSystemExample.UnitTests.csproj
	dotnet build src/PaymentsSystemExample.IntegrationTests/PaymentsSystemExample.IntegrationTests.csproj

all-tests: unit-test, integration-test

put-multi:
	http PUT localhost:5000/api/v1/payment X-CultureCode:en-GB Content-Type:application/json < test-data/multiple_payments_payload_test.json

put-single:
	http PUT localhost:5000/api/v1/payment X-CultureCode:en-GB Content-Type:application/json < test-data/one_payment_payload_test.json

unit-test:
	dotnet test src/PaymentsSystemExample.UnitTests/PaymentsSystemExample.UnitTests.csproj

integration-test:
	dotnet test src/PaymentsSystemExample.IntegrationTests/PaymentsSystemExample.IntegrationTests.csproj

auto-test:
	rg --files -tcsharp | entr make unit-test
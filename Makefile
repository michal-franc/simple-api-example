.PHONY: unit-test, auto-test, integration-test, dynamodb, all-tests, put-multi, put-single, example


example: docker-build start-docker put-single put-multi list-payments get-payment delete-payment stop

docker-build: 
	docker build -t michal-franc-payments .

start-docker: dynamodb
	cd test-infra && docker-compose up -d payments-system-example
	sleep 2

start-local: dynamodb
	dotnet run --project src/PaymentsSystemExample.Api/PaymentsSystemExample.Api.csproj

stop:	
	cd test-infra && docker-compose down

build:
	dotnet build src/PaymentsSystemExample.Api/PaymentsSystemExample.Api.csproj
	dotnet build src/PaymentsSystemExample.Domain/PaymentsSystemExample.Domain.csproj
	dotnet build src/PaymentsSystemExample.UnitTests/PaymentsSystemExample.UnitTests.csproj
	dotnet build src/PaymentsSystemExample.IntegrationTests/PaymentsSystemExample.IntegrationTests.csproj

dynamodb:
	cd test-infra && docker-compose up -d localstack
	- aws --endpoint-url=http://localhost:4569 dynamodb create-table --table-name payments --attribute-definitions AttributeName=PaymentId,AttributeType=S --key-schema AttributeName=PaymentId,KeyType=HASH  --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5

tests: unit-test dynamodb integration-test stop

delete-payment:
	curl -X DELETE localhost:5000/api/v1/payment/4ee3a8d8-ca7b-4290-a52c-dd5b6165ec43

get-payment:
	curl -X GET localhost:5000/api/v1/payment/4ee3a8d8-ca7b-4290-a52c-dd5b6165ec43

list-payments:
	curl -X GET localhost:5000/api/v1/payments/743d5b63-8e6f-432e-a8fa-c5d8d2ee5fcb

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

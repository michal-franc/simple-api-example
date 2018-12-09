#!/bin/bash
aws --endpoint-url=http://localhost:4569 dynamodb create-table --table-name payments --attribute-definitions AttributeName=PaymentId,AttributeType=S --key-schema AttributeName=PaymentId,KeyType=HASH  --provisioned-throughput ReadCapacityUnits=5,WriteCapacityUnits=5

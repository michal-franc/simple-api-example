.PHONY: put-multi, put-single

put-multi:
	http PUT localhost:5000/api/payment X-CultureCode:en-GB Content-Type:application/json < multiple_payments_payload_test.json

put-single:
	http PUT localhost:5000/api/payment X-CultureCode:en-GB Content-Type:application/json < one_payment_payload_test.json

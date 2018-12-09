using System;
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using Xunit;
using FluentAssertions;
using PaymentsSystemExample.Domain.Adapters;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;

namespace PaymentsSystemExample.UnitTests
{
    // TODO: I Would similar tests for FX, Charges informatioon and sender charges test
    // But omiting it now
    public class WhenParsingParty
    {
        [Theory]
        [InlineData("beneficiary")]
        [InlineData("debtor")]
        [InlineData("sponsor")]
        public void ForParty_TheFieldsAreCorrectlyMapped(string partyType)
        {
            var sut = new PaymentParserJsonGB();
            var testAccountName = "accountName";
            var testAccountNumber = "accountNumber";
            var testAccountNumberCode = "accountNumberCode";
            var testAccountType = 9;
            var testAddress = "address";
            var testBankId = "bankId";
            var testBankIdCode = "bankCode";
            var testName = "name";

            var testJson = $@"{{
                'data': [{{
                    'attributes': {{
                        '{partyType}_party': {{
                            'account_name': '{testAccountName}',
                            'account_number': '{testAccountNumber}',
                            'account_number_code': '{testAccountNumberCode}',
                            'account_type': '{testAccountType}',
                            'address': '{testAddress}',
                            'bank_id': '{testBankId}',
                            'bank_id_code': '{testBankIdCode}',
                            'name': '{testName}'
                        }}
                    }}
                }}]
            }}";

            var resultPayment = sut.Parse(testJson);

            sut.HasErrors.Should().Be(false);

            var attributes = resultPayment.First().Attributes;

            Party testParty = null;

            // this is not perfect but adding refleciton here would be an overkill
            // This test is maybe overcomplicated and should be split
            switch(partyType)
            {
                case "beneficiary": 
                    testParty = attributes.Beneficiary;
                    break;
                case "debtor": 
                    testParty = attributes.Debtor;
                    break;
                case "sponsor": 
                    testParty = attributes.Sponsor;
                    break;
            }

            testParty.AccountName.Should().Be(testAccountName);
            testParty.AccountNumber.Should().Be(testAccountNumber);
            testParty.AccountType.Should().Be(testAccountType);
            testParty.AccountNumberCode.Should().Be(testAccountNumberCode);
            testParty.Address.Should().Be(testAddress);
            testParty.BankId.Should().Be(testBankId);
            testParty.BankIdCode.Should().Be(testBankIdCode);
        }
    }

    // I am not validating domain here just checking if this fields are correctly mapped
    // This kind of test could be handled by json schema
    // This test could be an overkill but is also usefull to make sure that we follow the contract.
    public class WhenParsingPaymentStringBasedFields
    {
        // This is one method to test multiple parsing values
        [Theory]
        [InlineData("currency", "Currency")]
        [InlineData("end_to_end_reference", "E2EReference")]
        [InlineData("numeric_reference", "NumericReference")]
        [InlineData("payment_id", "Id")]
        [InlineData("payment_purpose", "Purpose")]
        [InlineData("payment_scheme", "Scheme")]
        [InlineData("payment_type", "Type")]
        [InlineData("reference", "Reference")]
        [InlineData("scheme_payment_sub_type", "SchemeSubType")]
        [InlineData("scheme_payment_type", "SchemeType")]
        public void TheFieldsAreCorrectlyParsed(string sourceField, string targetField)
        {
            var sut = new PaymentParserJsonGB();
            var expectedValue = $"test - data {targetField}";

            var testJson = $@"{{
                'data': [{{
                    'attributes': {{
                        '{sourceField}': '{expectedValue}'
                    }}
                }}]
            }}";

            var resultPayment = sut.Parse(testJson);

            sut.HasErrors.Should().Be(false);
            expectedValue.Should().Be(TestHelper.GetStringValueUsingFieldName(targetField, resultPayment.First().Attributes));
        }
    }

    // Based on the api reference
    // http://api-docs.form3.tech/api.html#transaction-api-payments-create
    // We expect date in format YYYY-MM-DD
    // We need to validate it and throw an error if user specifries different date
    // For instance with hours - assuming that we will process this payment in an hour
    // At the beggingign with date i was consfused why there is no timezone but check to the api revealed that
    // Format is deliberately mentioned
    public class WhenParsingPaymentProcessingDateFromJsonTests
    {
        [Fact]
        public void AndDateTimeFormatIsValid_ThenReturnPayment_AndNoParsingErrors()
        {
            var sut = new PaymentParserJsonGB();
            var expectedDate = "2017-01-18";
            var expectedFormat = "yyyy-MM-dd";

            var testJson = $@"{{
                'data': [{{
                    'attributes': {{
                        'processing_date': '{expectedDate}'
                    }}
                }}]
            }}";

            var resultPayment = sut.Parse(testJson);

            sut.HasErrors.Should().Be(false);
            expectedDate.Should().Be(resultPayment.First().Attributes.ProcessingDate.ToString(expectedFormat));
        }

        [Fact]
        public void AndDateTimeWithMinutesAndHours_IsInValid_ThenReturnPayment_AndParsingErrors()
        {
            var sut = new PaymentParserJsonGB();
            var expectedDate = "2017-01-18 10:10:10";

            var testJson = $@"{{
                'data': [{{
                    'attributes': {{
                        'processing_date': '{expectedDate}'
                    }}
                }}]
            }}";

            var resultPayment = sut.Parse(testJson);
            sut.HasErrors.Should().Be(true);
        }

        [Fact]
        public void AndDateInDiffernetFormat_ThenReturnPayment_AndParsingErrors()
        {
            var sut = new PaymentParserJsonGB();
            var expectedDate = "2017-18-01";

            var testJson = $@"{{
                'data': [{{
                    'attributes': {{
                        'processing_date': '{expectedDate}'
                    }}
                }}]
            }}";

            var resultPayment = sut.Parse(testJson);
            sut.HasErrors.Should().Be(true);
        }
    }

    // Based on  http://api-docs.form3.tech/api.html#transaction-api-payments
    // api accepts only decimal points
    // but to show how I would create i18n parser i added option for specyfing culture code
    // in the base api i will just use culture with decimal point to force it on the user
    public class WhenMappingPaymentAmountFromJsonTests
    {
        [Fact]
        public void AndAmountIsValid_ThenReturnPayment_AndNoParsingErrors()
        {
            var sut = new PaymentParserJsonGB();
            var expectedAmount = 100.21m;

            var testJson = $@"{{
                'data': [{{
                    'attributes': {{
                        'amount': '{expectedAmount}'
                    }}
                }}]
            }}";

            var resultPayment = sut.Parse(testJson);
            expectedAmount.Should().Be(resultPayment.First().Attributes.Amount);
            sut.HasErrors.Should().Be(false);
        }

        [Fact]
        public void AndInvalidDecimalSeparator_ThenReturnPayment_WithParsingErrors()
        {
            var sut = new PaymentParserJsonGB();
            var expectedAmount = "100,21";

            var testJson = $@"{{
                'data': [{{
                    'attributes':{{
                        'amount': '{expectedAmount}'
                    }}
                }}]
            }}";

            var resultPayment = sut.Parse(testJson);
            sut.HasErrors.Should().Be(true);
        }

        [Fact]
        public void WithCommaSeparatedCulture_RetunrPayment_AndNoParsingErrors()
        {
            var testCulture = "nl-BE";
            var sut = new PaymentParserJson();
            var expectedAmount = "100,21";

            var testJson = $@"{{
                'data': [{{
                    'attributes':{{
                        'amount': '{expectedAmount}'
                    }}
                }}]
            }}";

            var resultPayment = sut.Parse(testJson, testCulture);
            sut.HasErrors.Should().Be(false);
            expectedAmount.Should().Be(resultPayment.First().Attributes.Amount.ToString(CultureInfo.CreateSpecificCulture(testCulture)));
        }
    }

    // Helper class for test so we dont have to write en-GB in all the tests
    internal class PaymentParserJsonGB : PaymentParserJson
    {
        public IEnumerable<Payment> Parse(string rawData)
        {
            return base.Parse(rawData, "en-GB");
        }
    }

    internal static class TestHelper 
    {
        public static string GetStringValueUsingFieldName(string fieldName, object obj)
        {
            var property = obj.GetType().GetProperty(fieldName);

            // For nicer message in failing unit tests
            if(property == null)
            {
                throw new Exception($"Property {fieldName} not found in {obj.GetType()}");
            }

            var value = property.GetValue(obj, null);

            if(value == null)
            {
                throw new Exception($"Value for field {fieldName} not found in {obj.GetType()}. Potential mapping problem on JsonProp level.");
            }

            return value.ToString();
        }
    }


}
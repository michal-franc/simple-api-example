using System;
using System.Linq;
using System.Globalization;
using Xunit;
using FluentAssertions;
using PaymentsSystemExample.Domain.Adapters;

namespace PaymentsSystemExample.UnitTests
{
    // Helper class for test so we dont have to write en-GB in all the tests
    public class PaymentMapperJsonGB : PaymentMapperJson
    {
        public PaymentMapperJsonGB(): base("en-GB") {}
    }

    // I am not validating domain here just checking if this fields are correctly mapped
    // This kind of test could be handled by json schema
    // This test could be an overkill but is also usefull to make sure that we follow the contract.
    public class WhenMappingPaymentStringBasedFields
    {
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
        public void TheFieldsAreCorrectlyMapped(string sourceField, string targetField)
        {
            var sut = new PaymentMapperJsonGB();
            var expectedValue = "JustARandomValue";

            var testJson = $@"{{
                'data': [{{
                    'attributes': {{
                        '{sourceField}': '{expectedValue}'
                    }}
                }}]
            }}";

            var resultPayment = sut.Map(testJson);

            sut.HasErrors.Should().Be(false);
            expectedValue.Should().Be(this.GetStringValueUsingFieldName(targetField, resultPayment.First().PaymentInJson));
        }

        private string GetStringValueUsingFieldName(string fieldName, object obj)
        {
            var property = obj.GetType().GetProperty(fieldName);

            // For nicer message in unit tests
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

    // Based on the api reference
    // http://api-docs.form3.tech/api.html#transaction-api-payments-create
    // We expect date in format YYYY-MM-DD
    // We need to validate it and throw an error if user specifries different date
    // For instance with hours - assuming that we will process this payment in an hour

    // At the beggingign with date i was consfused why there is no timezone but check to the api revealeed that
    // Format is deliberately mentioned
    public class WhenMappingPaymentProcessingDateFromJsonTests
    {
        [Fact]
        public void AndDateTimeFormatIsValid_ThenReturnPayment_AndNoParsingErrors()
        {
            var sut = new PaymentMapperJsonGB();
            var expectedDate = "2017-01-18";
            var expectedFormat = "yyyy-MM-dd";

            var testJson = $@"{{
                'data': [{{
                    'attributes': {{
                        'processing_date': '{expectedDate}'
                    }}
                }}]
            }}";

            var resultPayment = sut.Map(testJson);

            sut.HasErrors.Should().Be(false);
            expectedDate.Should().Be(resultPayment.First().PaymentInJson.ProcessingDate.ToString(expectedFormat));
        }

        [Fact]
        public void AndDateTimeWithMinutesAndHours_IsInValid_ThenReturnPayment_AndParsingErrors()
        {
            var sut = new PaymentMapperJsonGB();
            var expectedDate = "2017-01-18 10:10:10";

            var testJson = $@"{{
                'data': [{{
                    'attributes': {{
                        'processing_date': '{expectedDate}'
                    }}
                }}]
            }}";

            var resultPayment = sut.Map(testJson);
            sut.HasErrors.Should().Be(true);
        }

        [Fact]
        public void AndDateInDiffernetFormat_ThenReturnPayment_AndParsingErrors()
        {
            var sut = new PaymentMapperJsonGB();
            var expectedDate = "2017-18-01";

            var testJson = $@"{{
                'data': [{{
                    'attributes': {{
                        'processing_date': '{expectedDate}'
                    }}
                }}]
            }}";

            var resultPayment = sut.Map(testJson);
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
            var sut = new PaymentMapperJsonGB();
            var expectedAmount = 100.21m;

            var testJson = $@"{{
                'data': [{{
                    'attributes': {{
                        'amount': '{expectedAmount}'
                    }}
                }}]
            }}";

            var resultPayment = sut.Map(testJson);
            expectedAmount.Should().Be(resultPayment.First().PaymentInJson.Amount);
            sut.HasErrors.Should().Be(false);
        }

        [Fact]
        public void AndInvalidDecimalSeparator_ThenReturnPayment_WithParsingErrors()
        {
            var sut = new PaymentMapperJsonGB();
            var expectedAmount = "100,21";

            var testJson = $@"{{
                'data': [{{
                    'attributes':{{
                        'amount': '{expectedAmount}'
                    }}
                }}]
            }}";

            var resultPayment = sut.Map(testJson);
            sut.HasErrors.Should().Be(true);
        }

        [Fact]
        public void WithCommaSeparatedCulture_RetunrPayment_AndNoParsingErrors()
        {
            var testCulture = "nl-BE";
            var sut = new PaymentMapperJson(testCulture);
            var expectedAmount = "100,21";

            var testJson = $@"{{
                'data': [{{
                    'attributes':{{
                        'amount': '{expectedAmount}'
                    }}
                }}]
            }}";

            var resultPayment = sut.Map(testJson);
            sut.HasErrors.Should().Be(false);
            expectedAmount.Should().Be(resultPayment.First().PaymentInJson.Amount.ToString(CultureInfo.CreateSpecificCulture(testCulture)));
        }
    }
}
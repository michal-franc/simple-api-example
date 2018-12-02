using System;
using System.Linq;
using System.Globalization;
using Xunit;
using PaymentsSystemExample.Domain.Adapters;

namespace PaymentsSystemExample.UnitTests
{

    // Helper class for test so we dont have to write en-GB in all the tests
    public class PaymentMapperJsonGB : PaymentMapperJson
    {
        public PaymentMapperJsonGB(): base("en-GB") {}
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

            var testJson = $@"{{
                'data': [{{
                    'attributes': {{
                        'processing_date': '{expectedDate}'
                    }}
                }}]
            }}";

            var resultPayment = sut.Map(testJson);
            Assert.Equal(expectedDate, resultPayment.First().ProcessingDate.ToString("yyyy-MM-dd"));
            Assert.False(sut.HasErrors);
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
            Assert.True(sut.HasErrors);
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
            Assert.True(sut.HasErrors);
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
            Assert.Equal(expectedAmount, resultPayment.First().Amount);
            Assert.False(sut.HasErrors);
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
            Assert.True(sut.HasErrors);
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
            Assert.False(sut.HasErrors);
            Assert.Equal(expectedAmount, resultPayment.First().Amount.ToString(CultureInfo.CreateSpecificCulture(testCulture)));
        }
    }
}
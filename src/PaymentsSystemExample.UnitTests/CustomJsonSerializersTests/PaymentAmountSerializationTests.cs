using Xunit;
using System;
using FluentAssertions;
using PaymentsSystemExample.Api.Services;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaymentsSystemExample.Domain.Adapters.CustomJsonConverters;

namespace PaymentsSystemExample.UnitTests.CustomJsonSerializersTests
{
    internal class AmountStub
    {
        [JsonProperty("amount")]
        [JsonConverter(typeof(PaymentAmountConverter))]
        public decimal Amount { get; set; }
    }

    internal class AmountNullableStub
    {
        [JsonProperty("amount")]
        [JsonConverter(typeof(PaymentAmountConverter))]
        public decimal? Amount { get; set; }
    }

    internal class AmountIncorrectFieldTypeStub
    {
        [JsonProperty("amount")]
        [JsonConverter(typeof(PaymentAmountConverter))]
        public string Amount { get; set; }
    }

    public class AmountConverterSerializationTests
    {
        [Fact]
        public void WhenSerializingAmount_IGetCorrectValue()
        {
            var testData = new AmountStub
            { 
                Amount = 10.11m
            };

            var serializedData = JsonConvert.SerializeObject(testData);

            serializedData.Should().Be("{\"amount\":\"10.11\"}");
        }

        [Fact]
        public void WhenSerializingNegativeAmount_IGetCorrectValue()
        {
            var testData = new AmountStub
            { 
                Amount = -10.11m
            };

            var serializedData = JsonConvert.SerializeObject(testData);

            serializedData.Should().Be("{\"amount\":\"-10.11\"}");
        }

        [Fact]
        public void WhenSerializingNullableAmount_IGetCorrectlyFormattedValue()
        {
            var testData = new AmountNullableStub
            { 
                Amount = 10.11m
            };

            var serializedData = JsonConvert.SerializeObject(testData);

            serializedData.Should().Be("{\"amount\":\"10.11\"}");
        }

        [Fact]
        public void WhenSerializingNullableAmount_AndTheValueIsNull_IGetCorrectlyFormattedValue()
        {
            var testData = new AmountNullableStub
            { 
                Amount = null
            };

            var serializedData = JsonConvert.SerializeObject(testData);

            serializedData.Should().Be("{\"amount\":null}");
        }

        [Fact]
        public void WhenSerializingIncorrectAmount_IGetException()
        {
            var testData = new AmountIncorrectFieldTypeStub
            { 
                Amount = "10.11"
            };

            Assert.Throws<JsonSerializationException>(() => JsonConvert.SerializeObject(testData));
        }
    }
}
using Xunit;
using System;
using FluentAssertions;
using PaymentsSystemExample.Api.Services;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PaymentsSystemExample.Domain.Adapters.CustomJsonConverters;

namespace PaymentsSystemExample.UnitTests
{
    internal class ProcessingDateTimeStub
    {
        [JsonProperty("processing_date")]
        [JsonConverter(typeof(ProcessingDateConverter))]
        public DateTime ProcessingDate { get; set; }
    }

    internal class ProcessingNullableDateTimeStub
    {
        [JsonProperty("processing_date")]
        [JsonConverter(typeof(ProcessingDateConverter))]
        public DateTime? ProcessingDate { get; set; }
    }

    internal class IncorrectFieldTypeStub
    {
        [JsonProperty("processing_date")]
        [JsonConverter(typeof(ProcessingDateConverter))]
        public string ProcessingDate { get; set; }
    }

    public class ProcessingDateConverterSerializationTests
    {
        [Fact]
        public void WhenSerializingProcessingDate_IGetCorrectlyFormattedValue()
        {
            var testData = new ProcessingDateTimeStub
            { 
                ProcessingDate = new DateTime(2018, 5, 15)
            };

            var serializedData = JsonConvert.SerializeObject(testData);

            serializedData.Should().Be("{\"processing_date\":\"2018-05-15\"}");
        }

        [Fact]
        public void WhenSerializingNullableProcessingDate_IGetCorrectlyFormattedValue()
        {
            var testData = new ProcessingNullableDateTimeStub
            { 
                ProcessingDate = new DateTime(2018, 5, 15)
            };

            var serializedData = JsonConvert.SerializeObject(testData);

            serializedData.Should().Be("{\"processing_date\":\"2018-05-15\"}");
        }

        [Fact]
        public void WhenSerializingNullableProcessingDate_AndTheValueIsNull_IGetCorrectlyFormattedValue()
        {
            var testData = new ProcessingNullableDateTimeStub
            { 
                ProcessingDate = null
            };

            var serializedData = JsonConvert.SerializeObject(testData);

            serializedData.Should().Be("{\"processing_date\":null}");
        }

        [Fact]
        public void WhenSerializingIncorrectProcessingDate_IGetException()
        {
            var testData = new IncorrectFieldTypeStub
            { 
                ProcessingDate = "2018-05-15"
            };

            Assert.Throws<JsonSerializationException>(() => JsonConvert.SerializeObject(testData));
        }
    }
}
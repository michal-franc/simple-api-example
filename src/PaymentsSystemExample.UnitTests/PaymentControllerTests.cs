using PaymentsSystemExample.Api.Controllers;
using FluentAssertions;
using Xunit;
using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentsSystemExample.Api.Services;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PaymentsSystemExample.UnitTests
{
    public class PaymentControllerTests_WhenCallingPost
    {
        [Fact]
        public async Task AndThereIsAValidPayment_Return200_AndUpdatePayment()
        {
            var cultureCode = "en-GB";
            var rawPayment = string.Empty;

            var noValidationErrors = new ValidationErrors();
            var paymentServiceMock = new Mock<IPaymentService>();
            paymentServiceMock.Setup(x => x.UpdatePayments(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(noValidationErrors);

            var sut = new PaymentController(paymentServiceMock.Object);

            var result = await sut.Post(rawPayment, cultureCode);

            result.Should().BeOfType<OkResult>();
        }

        // We are just mocking this scenario and testing controller behaviour due to validation errors
        [Fact]
        public async Task AndThereIsAInvalidPayment_Return400_AndReturnValidationErrors()
        {
            var cultureCode = "en-GB";
            var rawPayment = string.Empty;

            var validationErrors = new ValidationErrors();
            validationErrors.AddParsingError("amount incorrect value");

            var paymentServiceMock = new Mock<IPaymentService>();
            paymentServiceMock.Setup(x => x.UpdatePayments(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(validationErrors);

            var sut = new PaymentController(paymentServiceMock.Object);

            IActionResult result = await sut.Post(rawPayment, cultureCode);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task AndThereIsNoCultureCode_ThenBadRequest()
        {
            var cultureCode = string.Empty;
            var rawPayment = string.Empty;

            var noValidationErrors = new ValidationErrors();
            var paymentServiceMock = new Mock<IPaymentService>();

            var sut = new PaymentController(paymentServiceMock.Object);

            var result = await sut.Post(rawPayment, cultureCode);

            result.Should().BeOfType<BadRequestObjectResult>();

            var badRequestObjectResult = (BadRequestObjectResult)result;
            badRequestObjectResult.Value.ToString().Should().Contain("X-CultureCode");
        }
    }

    public class PaymentControllerTests_WhenCallingPut
    {
        [Fact]
        public async Task AndThereIsAValidPayment_Return200_AndSavePayment()
        {
            var cultureCode = "en-GB";
            var rawPayment = string.Empty;

            var noValidationErrors = new ValidationErrors();
            var paymentServiceMock = new Mock<IPaymentService>();
            paymentServiceMock.Setup(x => x.CreatePayments(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(noValidationErrors);

            var sut = new PaymentController(paymentServiceMock.Object);

            var result = await sut.Put(rawPayment, cultureCode);

            result.Should().BeOfType<OkResult>();
        }

        // We are just mocking this scenario and testing controller behaviour due to validation errors
        [Fact]
        public async Task AndThereIsAInvalidPayment_Return400_AndReturnValidationErrors()
        {
            var cultureCode = "en-GB";
            var rawPayment = string.Empty;

            var validationErrors = new ValidationErrors();
            validationErrors.AddParsingError("amount: incorrect value");

            var paymentServiceMock = new Mock<IPaymentService>();
            paymentServiceMock.Setup(x => x.CreatePayments(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(validationErrors);

            var sut = new PaymentController(paymentServiceMock.Object);

            IActionResult result = await sut.Put(rawPayment, cultureCode);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task AndThereIsNoCultureCode_ThenBadRequest()
        {
            var cultureCode = string.Empty;
            var rawPayment = string.Empty;

            var noValidationErrors = new ValidationErrors();
            var paymentServiceMock = new Mock<IPaymentService>();

            var sut = new PaymentController(paymentServiceMock.Object);

            var result = await sut.Put(rawPayment, cultureCode);
            result.Should().BeOfType<BadRequestObjectResult>();

            var badRequestObjectResult = (BadRequestObjectResult)result;
            badRequestObjectResult.Value.ToString().Should().Contain("X-CultureCode");
        }
    }

    public class PaymentControllerTests_WhenCallingDelete
    {
        [Fact]
        public async Task AndThereIsNoPayment_ThenReturn404()
        {
            var nonExistingPaymentId = Guid.NewGuid();
            var paymentServiceMock = new Mock<IPaymentService>();
            paymentServiceMock.Setup(x => x.DeletePayment(nonExistingPaymentId)).ReturnsAsync(false);
            var sut = new PaymentController(paymentServiceMock.Object);

            var result = await sut.Delete(nonExistingPaymentId.ToString());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AndThereIsPayment_ThenReturn200()
        {
            var existingPaymentId = Guid.NewGuid();
            var paymentServiceMock = new Mock<IPaymentService>();
            paymentServiceMock.Setup(x => x.DeletePayment(existingPaymentId)).ReturnsAsync(true);
            var sut = new PaymentController(paymentServiceMock.Object);

            var result = await sut.Delete(existingPaymentId.ToString());

            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async Task AndTheIdIsIncorrect_ThenReturnBadRequest()
        {
            var nonGuidId = "testtesttest";
            var sut = new PaymentController(null);

            var result = await sut.Delete(nonGuidId);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }

    public class PaymentControllerTests_WhenCallingGet
    {
        [Fact]
        public async Task AndThereIsNoPayment_ThenReturn404()
        {
            var nonExistingPaymentId = Guid.NewGuid();
            var paymentServiceMock = new Mock<IPaymentService>();
            var sut = new PaymentController(paymentServiceMock.Object);

            var result = await sut.Get(nonExistingPaymentId.ToString());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task AndThereIsAPayment_ThenReturn200_and_thePayment()
        {
            var existingPaymentId = Guid.NewGuid();
            var paymentServiceMock = new Mock<IPaymentService>();
            paymentServiceMock.Setup(x => x.GetPayment(existingPaymentId)).ReturnsAsync(new Payment { Id = existingPaymentId });
            var sut = new PaymentController(paymentServiceMock.Object);

            var result = await sut.Get(existingPaymentId.ToString());

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task AndTheIdIsIncorrect_ThenReturnBadRequest()
        {
            var nonGuidId = "testtesttest";
            var sut = new PaymentController(null);

            var result = await sut.Get(nonGuidId);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
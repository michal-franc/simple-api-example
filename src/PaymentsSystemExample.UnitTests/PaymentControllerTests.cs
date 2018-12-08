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

namespace PaymentsSystemExample.UnitTests
{
    public class PaymentControllerTests_WhenCallingPost
    {
        [Fact]
        public void AndThereIsAValidPayment_Return200_AndUpdatePayment()
        {
            var noValidationErrors = new ValidationErrors();
            var paymentServiceMock = new Mock<IPaymentService>();
            paymentServiceMock.Setup(x => x.UpdatePayment(It.IsAny<string>(), It.IsAny<string>())).Returns(noValidationErrors);

            var sut = new PaymentController(paymentServiceMock.Object);

            var result = sut.Post(string.Empty);

            result.Should().BeOfType<OkResult>();
        }

        // We are just mocking this scenario and testing controller behaviour due to validation errors
        [Fact]
        public void AndThereIsAInvalidPayment_Return400_AndReturnValidationErrors()
        {
            var validationErrors = new ValidationErrors();
            validationErrors.Add("amount", "incorrect value");

            var paymentServiceMock = new Mock<IPaymentService>();
            paymentServiceMock.Setup(x => x.UpdatePayment(It.IsAny<string>(), It.IsAny<string>())).Returns(validationErrors);

            var sut = new PaymentController(paymentServiceMock.Object);

            IActionResult result = sut.Post(string.Empty);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }

    public class PaymentControllerTests_WhenCallingPut
    {
        [Fact]
        public void AndThereIsAValidPayment_Return200_AndSavePayment()
        {
            var noValidationErrors = new ValidationErrors();
            var paymentServiceMock = new Mock<IPaymentService>();
            paymentServiceMock.Setup(x => x.CreatePayment(It.IsAny<string>(), It.IsAny<string>())).Returns(noValidationErrors);

            var sut = new PaymentController(paymentServiceMock.Object);

            var result = sut.Put(string.Empty);

            result.Should().BeOfType<OkResult>();
        }

        // We are just mocking this scenario and testing controller behaviour due to validation errors
        [Fact]
        public void AndThereIsAInvalidPayment_Return400_AndReturnValidationErrors()
        {
            var validationErrors = new ValidationErrors();
            validationErrors.Add("amount", "incorrect value");

            var paymentServiceMock = new Mock<IPaymentService>();
            paymentServiceMock.Setup(x => x.CreatePayment(It.IsAny<string>(), It.IsAny<string>())).Returns(validationErrors);

            var sut = new PaymentController(paymentServiceMock.Object);

            IActionResult result = sut.Put(string.Empty);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }

    public class PaymentControllerTests_WhenCallingDelete
    {
        [Fact]
        public void AndThereIsNoPayment_ThenReturn404()
        {
            var nonExistingPaymentId = Guid.NewGuid();
            var paymentServiceMock = new Mock<IPaymentService>();
            paymentServiceMock.Setup(x => x.DeletePayment(nonExistingPaymentId)).Returns(false);
            var sut = new PaymentController(paymentServiceMock.Object);

            var result = sut.Delete(nonExistingPaymentId.ToString());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void AndThereIsPayment_ThenReturn200()
        {
            var existingPaymentId = Guid.NewGuid();
            var paymentServiceMock = new Mock<IPaymentService>();
            paymentServiceMock.Setup(x => x.DeletePayment(existingPaymentId)).Returns(true);
            var sut = new PaymentController(paymentServiceMock.Object);

            var result = sut.Delete(existingPaymentId.ToString());

            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public void AndTheIdIsIncorrect_ThenReturnBadRequest()
        {
            var nonGuidId = "testtesttest";
            var sut = new PaymentController(null);

            var result = sut.Delete(nonGuidId);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }

    public class PaymentControllerTests_WhenCallingGet
    {
        [Fact]
        public void AndThereIsNoPayment_ThenReturn404()
        {
            var nonExistingPaymentId = Guid.NewGuid();
            var paymentServiceMock = new Mock<IPaymentService>();
            var sut = new PaymentController(paymentServiceMock.Object);

            var result = sut.Get(nonExistingPaymentId.ToString());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public void AndThereIsAPayment_ThenReturn200_and_thePayment()
        {
            var existingPaymentId = Guid.NewGuid();
            var paymentServiceMock = new Mock<IPaymentService>();
            paymentServiceMock.Setup(x => x.GetPayment(existingPaymentId)).Returns(new Payment { Id = existingPaymentId });
            var sut = new PaymentController(paymentServiceMock.Object);

            var result = sut.Get(existingPaymentId.ToString());

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public void AndTheIdIsIncorrect_ThenReturnBadRequest()
        {
            var nonGuidId = "testtesttest";
            var sut = new PaymentController(null);

            var result = sut.Get(nonGuidId);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
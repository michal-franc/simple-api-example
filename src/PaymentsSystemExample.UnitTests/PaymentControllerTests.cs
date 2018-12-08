using PaymentsSystemExample.Api.Controllers;
using FluentAssertions;
using Xunit;
using System;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentsSystemExample.Api.Services;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;

namespace PaymentsSystemExample.UnitTests
{
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
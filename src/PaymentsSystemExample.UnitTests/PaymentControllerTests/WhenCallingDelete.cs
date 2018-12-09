
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Moq;
using Xunit;
using FluentAssertions;

using PaymentsSystemExample.Api.Controllers;
using PaymentsSystemExample.Api.Services;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;

namespace PaymentsSystemExample.UnitTests.PaymentControllerTests
{
    public class PaymentControllerTests_WhenCallingDelete
    {
        [Fact]
        public async Task AndThereIsExeption_ThenReturn500()
        {
            var paymentServiceMock = new Mock<IPaymentService>();
            paymentServiceMock.Setup(x => x.DeletePayment(It.IsAny<Guid>())).ThrowsAsync(new Exception());

            var sut = new PaymentController(paymentServiceMock.Object);

            var result = await sut.Delete(Guid.NewGuid().ToString());

            result.Should().BeOfType<StatusCodeResult>();

            var castedResult = (StatusCodeResult)result;
            castedResult.StatusCode.Should().Be(500);
        }

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
}
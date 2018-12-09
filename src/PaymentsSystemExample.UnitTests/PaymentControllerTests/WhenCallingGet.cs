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
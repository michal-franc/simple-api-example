using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Moq;
using Xunit;
using FluentAssertions;

using PaymentsSystemExample.Api.Controllers;
using PaymentsSystemExample.Api.Services;
using PaymentsSystemExample.Domain.Adapters.JsonObjects;

namespace PaymentsSystemExample.UnitTests.PaymentsControllerTests
{
    public class PaymentsControllerTests_WhenCallingGet
    {
        [Fact]
        public async Task AndThereIsExeption_ThenReturn500()
        {
            var paymentServiceMock = new Mock<IPaymentService>();
            paymentServiceMock.Setup(x => x.GetPayments(It.IsAny<Guid>())).ThrowsAsync(new Exception());

            var sut = new PaymentsController(paymentServiceMock.Object);

            var result = await sut.Get(Guid.NewGuid().ToString());

            result.Should().BeOfType<StatusCodeResult>();

            var castedResult = (StatusCodeResult)result;
            castedResult.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task AndThereAreNoPaymentsForThisOrganisation_ThenReturn404()
        {
            var nonExistingOrgId = Guid.NewGuid();
            var paymentServiceMock = new Mock<IPaymentService>();
            var sut = new PaymentsController(paymentServiceMock.Object);

            var result = await sut.Get(nonExistingOrgId.ToString());

            result.Should().BeOfType<NotFoundResult>();
        }
        
        [Fact]
        public async Task AndThereArePayments_ThenReturnThem()
        {
            var nonExistingOrgId = Guid.NewGuid();
            var paymentServiceMock = new Mock<IPaymentService>();
            paymentServiceMock.Setup(x => x.GetPayments(nonExistingOrgId)).ReturnsAsync(new [] { new Payment()});

            var sut = new PaymentsController(paymentServiceMock.Object);

            var result = await sut.Get(nonExistingOrgId.ToString());

            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task AndTheOrgIdIsIncorrect_ThenReturnBadRequest()
        {
            var nonGuidId = "testtesttest";
            var sut = new PaymentsController(null);

            var result = await sut.Get(nonGuidId);

            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
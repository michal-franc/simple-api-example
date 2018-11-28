using System;
using Xunit;

namespace PaymentsSystemExample.UnitTests
{
    public class BasicUnitTest
    {
        [Fact]
        public void TestTrue()
        {
            Assert.True(true);
        }

        [Fact]
        public void TestFail()
        {
            Assert.True(false);
        }
    }
}

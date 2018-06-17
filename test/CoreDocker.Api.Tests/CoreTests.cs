using CoreDocker.Api.Mappers;
using NUnit.Framework;

namespace CoreDocker.Api.Tests
{
    [TestFixture]
    public class CoreTests
    {
        [Test]
        public void AssertConfigurationIsValid_WhenCalled_ShouldBeValid()
        {
            MapApi.GetInstance();
            MapApi.AssertConfigurationIsValid();
        }
    }
}
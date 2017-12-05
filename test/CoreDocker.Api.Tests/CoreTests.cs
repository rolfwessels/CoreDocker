using CoreDocker.Api.Models.Mappers;
using NUnit.Framework;

namespace CoreDocker.Api.Tests
{
    [TestFixture]
    public class CoreTests
    {
        [Test]
        public void AssertConfigurationIsValid_WhenCalled_ShouldBeValid()
        {
            var instance = MapApi.GetInstance();
            MapApi.AssertConfigurationIsValid();
        }
    }
}
using CoreDocker.Api.Mappers;
using CoreDocker.Core.Framework.Mappers;
using NUnit.Framework;

namespace CoreDocker.Api.Tests.Startup
{
    [TestFixture]
    public class AutoMapperSetupTests
    {
        [Test]
        public void AssertConfigurationIsValid_OnMapCore_ShouldNotFail()
        {
            // assert
            MapCore.AssertConfigurationIsValid();
        }

        [Test]
        public void AssertConfigurationIsValid_OnMapApi_ShouldNotFail()
        {
            // assert
            MapApi.AssertConfigurationIsValid();
        }
    }
}
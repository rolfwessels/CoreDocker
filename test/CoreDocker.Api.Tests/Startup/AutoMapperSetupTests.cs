using CoreDocker.Api.Models.Mappers;
using CoreDocker.Core.Mappers;
using NUnit.Framework;

namespace CoreDocker.Api.Tests.Startup
{
    [TestFixture]
    public class AutoMapperSetupTests
    {
        [Test]
        public void AssertConfigurationIsValid_OnMapCore_ShouldNotFaile()
        {
            // assert
            MapCore.AssertConfigurationIsValid();
        }

        [Test]
        public void AssertConfigurationIsValid_OnMapApi_ShouldNotFaile()
        {
            // assert
            MapApi.AssertConfigurationIsValid();
        }
    }
}
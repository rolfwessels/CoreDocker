using NUnit.Framework;
using CoreDocker.Api.Models.Mappers;
using AutoMapper;

namespace CoreDocker.Core.Tests
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
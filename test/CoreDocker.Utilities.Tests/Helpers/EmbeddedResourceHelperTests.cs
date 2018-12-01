using System;
using CoreDocker.Utilities.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace CoreDocker.Utilities.Tests.Helpers
{
    [TestFixture]
    public class EmbeddedResourceHelperTests
    {
        [Test]
        public void ReadResource_GivenInvalidResource_ShouldThrowException()
        {
            // arrange
            const string path = "CoreDocker.Utilities.Tests.Resources.t1.txt";
            // action
            Action testCall = () => { EmbededResourceHelper.ReadResource(path, typeof(EmbeddedResourceHelperTests)); };
            // assert
            testCall.Should().Throw<ArgumentException>().WithMessage(
                "CoreDocker.Utilities.Tests.Resources.t1.txt resource does not exist in CoreDocker.Utilities.Tests assembly.");
        }

        [Test]
        public void ReadResource_GivenValidResource_ShouldReturnString()
        {
            // arrange
            const string path = "CoreDocker.Utilities.Tests.Resources.t.txt";
            // action
            var readResource = EmbededResourceHelper.ReadResource(path, typeof(EmbeddedResourceHelperTests));
            // assert
            readResource.Should().Be("sample");
        }
    }
}
using System;
using FluentAssertions;
using NUnit.Framework;
using CoreDocker.Utilities.Helpers;

namespace CoreDocker.Utilities.Tests.Helpers
{
    [TestFixture]
    public class EmbededResourceHelperTests
    {
        [Test]
        public void ReadResource_GivenValidResource_ShouldReturnString()
        {
            // arrange
            const string path = "CoreDocker.Utilities.Tests.Resources.t.txt";
            // action
            var readResource = EmbededResourceHelper.ReadResource(path,typeof (EmbededResourceHelperTests));
            // assert
            readResource.Should().Be("sample");
        }

        [Test]
        public void ReadResource_GivenInvalidResource_ShouldThrowException()
        {
            // arrange
            const string path = "CoreDocker.Utilities.Tests.Resources.t1.txt";
            // action
            Action testCall = () =>
            {
                EmbededResourceHelper.ReadResource(path, typeof (EmbededResourceHelperTests));
            };
            ;
            // assert

            testCall.Should().Throw<ArgumentException>().WithMessage("CoreDocker.Utilities.Tests.Resources.t1.txt resource does not exist in CoreDocker.Utilities.Tests assembly.");
        }


    }
}
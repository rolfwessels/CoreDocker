using System;
using NUnit.Framework;
using CoreDocker.Utilities.Helpers;
using FluentAssertions;

namespace CoreDocker.Utilities.Tests.Helpers
{
    [TestFixture]
    public class EnumHelperTests
    {
        [Test]
        public void method_GiventestingFor_Shouldresult()
        {
            // arrange
            Setup();
            // action
            var dayOfWeeks = EnumHelper.ToArray<DayOfWeek>();
            // assert
            dayOfWeeks.Should().Contain(DayOfWeek.Friday).And.HaveCount(7);
        }

        private void Setup()
        {
            
        }
    }

}
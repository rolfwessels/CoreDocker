using System;
using CoreDocker.Utilities.Helpers;
using FluentAssertions;
using NUnit.Framework;

namespace CoreDocker.Utilities.Tests.Helpers
{
    [TestFixture]
    public class EnumHelperTests
    {
        #region Setup/Teardown

        private void Setup()
        {
        }

        #endregion

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
    }
}
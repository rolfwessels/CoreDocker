using System.Linq;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Core.Framework.MessageUtil.Models;
using CoreDocker.Core.Tests.Helpers;
using CoreDocker.Dal.Models.Base;
using CoreDocker.Dal.Persistence;
using CoreDocker.Dal.Tests;
using FizzWare.NBuilder;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace CoreDocker.Core.Tests.Framework.BaseManagers
{
    public abstract class BaseTypedManagerTests<T> : BaseManagerTests where T : BaseDalModelWithId
    {
        [Test]
        public virtual async Task Get_WhenCalledWithId_ShouldReturnSingleRecord()
        {
            // arrange
            Setup();
            var addFake = Repository.AddFake();
            var guid = addFake.First().Id;
            // action
            var result = await Lookup.GetById(guid);
            // assert
            result?.Id.Should().Be(guid);
        }

        [Test]
        public virtual async Task GetRecords_WhenCalled_ShouldReturnRecords()
        {
            // arrange
            Setup();
            const int expected = 2;
            Repository.AddFake(expected);
            // action
            var result = await Lookup.Get();
            // assert
            result.Should().HaveCount(expected);
        }

        protected abstract IRepository<T> Repository { get; }

        protected virtual T SampleObject => Builder<T>.CreateNew().WithValidData().Build();

        protected abstract BaseLookup<T> Lookup { get; }
    }
}
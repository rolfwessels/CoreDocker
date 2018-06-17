using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDocker.Core.Framework.BaseManagers;
using CoreDocker.Core.Framework.MessageUtil.Models;
using FizzWare.NBuilder;
using FluentAssertions;
using CoreDocker.Core.Tests.Helpers;
using CoreDocker.Dal.Models;
using CoreDocker.Dal.Models.Base;
using CoreDocker.Dal.Persistance;
using CoreDocker.Utilities.Tests.TempBuildres;
using Moq;
using NUnit.Framework;

namespace CoreDocker.Core.Tests.Managers
{
    public abstract class BaseTypedManagerTests<T> : BaseManagerTests where T : BaseDalModelWithId
    {
        [Test]
        public virtual async Task Delete_WhenCalledWithExisting_ShouldCallMessageThatDataWasRemoved()
        {
            // arrange
            Setup();
            T project = Repository.AddFake().First();
            // action
            await Manager.Delete(project.Id);
            // assert
            _mockIMessenger.Verify(mc => mc.Send(It.Is<DalUpdateMessage<T>>(m => m.UpdateType == UpdateTypes.Removed)),
                                   Times.Once);
            Manager.GetById(project.Id).Result.Should().BeNull();
        }

        [Test]
        public virtual async Task GetRecords_WhenCalled_ShouldReturnRecords()
        {
            // arrange
            Setup();
            const int expected = 2;
            Repository.AddFake(expected);
            // action
            var result = await Manager.Get();
            // assert
            result.Should().HaveCount(expected);
        }

        [Test]
        public virtual async Task Get_WhenCalledWithId_ShouldReturnSingleRecord()
        {
            // arrange
            Setup();
            IList<T> addFake = Repository.AddFake();
            string guid = addFake.First().Id;
            // action
            T result = await Manager.GetById(guid);
            // assert
            result.Id.Should().Be(guid);
        }

        [Test]
        public virtual async Task Save_WhenCalledWithExisting_ShouldCallMessageThatDataWasUpdated()
        {
            // arrange
            Setup();
            T project = Repository.AddFake().First();
            // action
            await Manager.Save(project);
            // assert
            _mockIMessenger.Verify(mc => mc.Send(It.Is<DalUpdateMessage<T>>(m => m.UpdateType == UpdateTypes.Updated)),
                                   Times.Once);
            _mockIMessenger.Verify(
                mc => mc.Send(It.Is<DalUpdateMessage<T>>(m => m.UpdateType == UpdateTypes.Inserted)), Times.Never);
        }

        [Test]
        public virtual async Task Save_WhenCalledWith_ShouldCallMessageThatDataWasInserted()
        {
            // arrange
            Setup();
            T project = SampleObject;
            // action
            await Manager.Save(project);
            // assert
            _mockIMessenger.Verify(
                mc => mc.Send(It.Is<DalUpdateMessage<T>>(m => m.UpdateType == UpdateTypes.Inserted)), Times.Once);
            _mockIMessenger.Verify(mc => mc.Send(It.Is<DalUpdateMessage<T>>(m => m.UpdateType == UpdateTypes.Updated)),
                                   Times.Never);
        }

        [Test]
        public virtual async Task Save_WhenCalledWith_ShouldSaveTheRecord()
        {
            // arrange
            Setup();
            T project = SampleObject;
            // action
            T result = await Manager.Save(project);
            // assert
            Repository.Count().Result.Should().Be(1L);
            result.Should().NotBeNull();
        }

        [Test]
        public virtual async Task Save_WhenCalledWith_ShouldToLowerTheEmail()
        {
            // arrange
            Setup();
            T project = SampleObject;
            // action
            T result = await Manager.Save(project);
            // assert
            result.Id.Should().Be(project.Id);
        }

        protected abstract IRepository<T> Repository { get; }

        protected virtual T SampleObject => Builder<T>.CreateNew().WithValidData().Build();

        protected abstract BaseManager<T> Manager { get; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using Bumbershoot.Utilities.Helpers;
using CoreDocker.Dal.Models.Base;
using CoreDocker.Dal.Persistence;
using CoreDocker.Dal.Tests;
using FizzWare.NBuilder;

namespace CoreDocker.Core.Tests.Helpers
{
    public static class FakeRepoHelper
    {
        public static IList<T> AddFake<T>(this IRepository<T> repository, int size, Action<T> applyUpdate)
            where T : IBaseDalModel
        {
            var items = Builder<T>.CreateListOfSize(size).WithValidData().Build();
            items.OfType<IBaseDalModelWithId>().ForEach(x => x.Id = null!);
            return items
                .ForEach(applyUpdate)
                .Select(repository.Add)
                .Select(x => x.Result)
                .ToList();
        }

        public static IList<T> AddFake<T>(this IRepository<T> repository, int size = 5) where T : IBaseDalModel
        {
            return AddFake(repository, size, t => { });
        }

        public static T AddAFake<T>(this IRepository<T> repository) where T : IBaseDalModel
        {
            return AddFake(repository, 1).First();
        }


        public static T AddAFake<T>(this IRepository<T> repository, Action<T> applyUpdate) where T : IBaseDalModel
        {
            return AddFake(repository, 1, applyUpdate).First();
        }
    }
}
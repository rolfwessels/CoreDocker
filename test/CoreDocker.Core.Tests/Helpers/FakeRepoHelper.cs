using System.Collections.Generic;
using CoreDocker.Dal.Models.Interfaces;
using CoreDocker.Dal.Persistance;
using FizzWare.NBuilder;

namespace CoreDocker.Core.Tests.Helpers
{
    public static class FakeRepoHelper
    {
        public static IList<T> AddFake<T>(this IRepository<T> repository, int size = 5) where T : IBaseDalModel
        {
            var list = new List<T>();
            for (int i = 0; i < size; i++)
            {
                var withValidData = Builder<T>.CreateNew().WithValidData().Build();
                list.Add(repository.Add(withValidData).Result);
            }
            return list;
//            var repoItems = Builder<T>.CreateListOfSize(size).All().WithValidData().Build();
//            foreach (var item in repoItems)
//            {
//                repository.Add(item);
//            }
//            return repoItems;
        }
        
        public static T AddAFake<T>(this IRepository<T> repository) where T : IBaseDalModel
        {
            var withValidData = Builder<T>.CreateNew().WithValidData().Build();
            return repository.Add(withValidData).Result;
        }
    }
}
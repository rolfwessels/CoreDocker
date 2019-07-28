﻿using System;
using CoreDocker.Dal.Persistence;

namespace CoreDocker.Dal.InMemoryCollections
{
    public class InMemoryGeneralUnitOfWorkFactory : IGeneralUnitOfWorkFactory
    {
        private readonly InMemoryGeneralUnitOfWork _inMemoryGeneralUnitOfWork;

        public InMemoryGeneralUnitOfWorkFactory()
        {
            _inMemoryGeneralUnitOfWork = new InMemoryGeneralUnitOfWork();
        }

        public string NewId => Guid.NewGuid().ToString().Substring(0, 26);

        #region IGeneralUnitOfWorkFactory Members

        public IGeneralUnitOfWork GetConnection()
        {
            return _inMemoryGeneralUnitOfWork;
        }

        #endregion
    }
}

﻿using System.Collections.Generic;
using System.Threading.Tasks;
using CoreDocker.Shared.Models;

namespace CoreDocker.Shared.Interfaces.Base
{
    public interface IBaseStandardLookups<TDetails, TModelReference> : IBaseControllerLookups<TDetails, TModelReference>
    {
        Task<PagedResult<TModelReference>> GetPaged(string oDataQuery);
        Task<IEnumerable<TModelReference>> Get(string oDataQuery);
        Task<IEnumerable<TDetails>> GetDetail(string oDataQuery);
        Task<PagedResult<TDetails>> GetDetailPaged(string oDataQuery);
    }
}
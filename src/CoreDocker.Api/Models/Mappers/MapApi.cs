using System;
using AutoMapper;
using MainSolutionTemplate.Core.MessageUtil.Models;
using MainSolutionTemplate.Shared.Models;
using MainSolutionTemplate.Shared.Models.Enums;

namespace MainSolutionTemplate.Api.Models.Mappers
{
	public static partial class MapApi
	{
        static MapApi()
        {
            Mapper.Initialize(cfg => {
                MapUserModel(cfg);
                MapProjectModel(cfg);
            });
            

        }

	    public static void EnsureInitialized()
	    {

	    }


	    public static ValueUpdateModel<TModel> ToValueUpdateModel<T, TModel>(this DalUpdateMessage<T> updateMessage)
		{
			return new ValueUpdateModel<TModel>(Mapper.Map<T, TModel>(updateMessage.Value), (UpdateTypeCodes) updateMessage.UpdateType);
		}
	}
}
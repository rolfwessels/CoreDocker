using AutoMapper;
using CoreDocker.Core.MessageUtil.Models;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Enums;
using log4net;
using CoreDocker.Dal.Models;
using System;

namespace CoreDocker.Api.Models.Mappers
{
    public static partial class MapApi
	{
	    private static readonly ILog _log = LogManager.GetLogger<Application>();

        static MapApi()
        {
            Mapper.Initialize(cfg => {
                MapUserModel(cfg);
                MapProjectModel(cfg);
                _log.Warn("casd");
            });
            
          
        }

        public static void Initialize()
        {
        }

        public static IMapper GetInstance() => Mapper.Instance;


        public static ValueUpdateModel<TModel> ToValueUpdateModel<T, TModel>(this DalUpdateMessage<T> updateMessage)
		{
			return new ValueUpdateModel<TModel>(Mapper.Instance.Map<T, TModel>(updateMessage.Value), (UpdateTypeCodes) updateMessage.UpdateType);
		}

        public static void AssertConfigurationIsValid()
        {
            Mapper.AssertConfigurationIsValid();
        }
    }
}
using AutoMapper;
using CoreDocker.Shared.Models;
using CoreDocker.Shared.Models.Enums;
using CoreDocker.Dal.Models;
using System;
using CoreDocker.Core.Framework.MessageUtil.Models;
using CoreDocker.Utilities.FakeLogging;

namespace CoreDocker.Api.Models.Mappers
{
    public static partial class MapApi
	{
	    private static readonly ILog _log = LogManager.GetLogger<Application>();
        private static readonly Lazy<IMapper> _mapper;
        static MapApi()
        {
            _mapper = new Lazy<IMapper>(InitializeMapping);
        }

        private static IMapper InitializeMapping()
        {
            
            var config = new MapperConfiguration(cfg => {
                MapUserModel(cfg);
                MapProjectModel(cfg);
                _log.Warn("casd");
            });
            return config.CreateMapper();
        }

        public static IMapper GetInstance()
        {
            return _mapper.Value;
        }


        public static ValueUpdateModel<TModel> ToValueUpdateModel<T, TModel>(this DalUpdateMessage<T> updateMessage)
		{
			return new ValueUpdateModel<TModel>(Mapper.Instance.Map<T, TModel>(updateMessage.Value), (UpdateTypeCodes) updateMessage.UpdateType);
		}

        public static void AssertConfigurationIsValid()
        {
            GetInstance().ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
using AutoMapper;
using MainSolutionTemplate.Core.MessageUtil.Models;
using MainSolutionTemplate.Shared.Models;
using MainSolutionTemplate.Shared.Models.Enums;
using log4net;
using MainSolutionTemplate.Dal.Models;

namespace MainSolutionTemplate.Api.Models.Mappers
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

        public static IMapper GetInstance() => Mapper.Instance;


        public static ValueUpdateModel<TModel> ToValueUpdateModel<T, TModel>(this DalUpdateMessage<T> updateMessage)
		{
			return new ValueUpdateModel<TModel>(Mapper.Instance.Map<T, TModel>(updateMessage.Value), (UpdateTypeCodes) updateMessage.UpdateType);
		}
	}
}
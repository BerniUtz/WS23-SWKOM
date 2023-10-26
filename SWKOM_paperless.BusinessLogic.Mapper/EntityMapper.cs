using AutoMapper;
using SWKOM_paperless.BusinessLogic.Entities;
using Org.OpenAPITools.Models;

namespace SWKOM_paperless.BusinessLogic.Mapper
{
    public class EntityMapper : Profile
    {
        private readonly IMapper _mapper;

        public EntityMapper()
        {
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Entities.Correspondent, Org.OpenAPITools.Models.Correspondent>();
                cfg.CreateMap<Entities.DocTag, Org.OpenAPITools.Models.DocTag>();
                cfg.CreateMap<Entities.Document, Org.OpenAPITools.Models.Document>();
                cfg.CreateMap<Entities.DocumentType, Org.OpenAPITools.Models.DocumentType>();
                cfg.CreateMap<Entities.NewCorrespondent, Org.OpenAPITools.Models.NewCorrespondent>();
                cfg.CreateMap<Entities.NewDocumentType, Org.OpenAPITools.Models.NewDocumentType>();
                cfg.CreateMap<Entities.NewTag, Org.OpenAPITools.Models.NewTag>();
                cfg.CreateMap<Entities.UserInfo, Org.OpenAPITools.Models.UserInfo>();
            });

            _mapper = mapperConfig.CreateMapper();
        }

        public TDestination Map<TSource, TDestination>(TSource source)
        {
            return _mapper.Map<TSource, TDestination>(source);
        }
    }
}
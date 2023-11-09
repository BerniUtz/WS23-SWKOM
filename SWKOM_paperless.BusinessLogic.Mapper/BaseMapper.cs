using System;
using AutoMapper;
using Newtonsoft.Json;

namespace SWKOM_paperless.BusinessLogic.Mapper
{
    public interface IBaseMapper<Entity, Dto>
    {
        Entity DtoToEntity(Dto dto);
        Dto EntityToDto(Entity entity);

        T? Map<T>(JsonNullable<T>? value);
        JsonNullable<T> Map<T>(T value);
    }

    public class JsonNullable<T>
    {
        [JsonProperty("value")] public T? Value { get; private set; }

        [JsonProperty("isPresent")] public bool IsPresent => Value != null;

        public static JsonNullable<T> Of(T value)
        {
            return new JsonNullable<T> { Value = value };
        }

        public T Get()
        {
            if (!IsPresent)
            {
                throw new InvalidOperationException("Nullable value is not present");
            }
            return Value;
        }
    }

    public class BaseMapper<Entity, Dto> : IBaseMapper<Entity, Dto>
    {
        private readonly IMapper _mapper;

        public BaseMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Dto, Entity>();
                cfg.CreateMap<Entity, Dto>();
            });

            _mapper = config.CreateMapper();
        }

        public Entity DtoToEntity(Dto dto)
        {
            return _mapper.Map<Entity>(dto);
        }

        public Dto EntityToDto(Entity entity)
        {
            return _mapper.Map<Dto>(entity);
        }

        public T? Map<T>(JsonNullable<T>? value)
        {
            if (value == null || !value.IsPresent)
            {
                return default(T);
            }

            return value.Value;
        }

        public JsonNullable<T> Map<T>(T value)
        {
            return JsonNullable<T>.Of(value);
        }
    }
}
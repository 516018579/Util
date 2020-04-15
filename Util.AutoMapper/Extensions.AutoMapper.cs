using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Util.AutoMapper
{
    public static partial class AutoMapperExtensions
    {
        private static ConcurrentDictionary<MapType, MapperConfiguration> _mapperConfigurations = new ConcurrentDictionary<MapType, MapperConfiguration>();

        private static MapperConfiguration GetConfiguration(Type sourceType, Type destinationType)
        {
            return _mapperConfigurations.GetOrAdd(new MapType(sourceType, destinationType), x => new MapperConfiguration(cfg =>
            {
                cfg.CreateMap(x.SourceType, x.DestinationType);
                cfg.CreateMap(x.DestinationType, x.SourceType);
            }));
        }

        private static MapperConfiguration GetConfiguration<TSource, TDestination>()
        {
            return GetConfiguration(typeof(TSource), typeof(TDestination));
        }

        #region IQueryable
        public static IQueryable<TDestination> MapTo<TSource, TDestination>(this IQueryable<TSource> query,
           Action<IMappingExpression<TSource, TDestination>> destinationAction = null,
           Action<IMappingExpression<TDestination, TSource>> sourceAction = null,
           Action<IMapperConfigurationExpression> configAction = null)
        {
            var map = new MapperConfiguration(cfg =>
            {
                var destinationExpression = cfg.CreateMap<TSource, TDestination>();
                var sourceExpression = cfg.CreateMap<TDestination, TSource>();
                destinationAction?.Invoke(destinationExpression);
                sourceAction?.Invoke(sourceExpression);
                configAction?.Invoke(cfg);
            });

            return query.ProjectTo<TDestination>(map);
        }

        public static IQueryable<TDestination> MapTo<TDestination>(this IQueryable query)
        {
            var map = GetConfiguration(query.GetType().GetGenericArguments()[0], typeof(TDestination));

            return query.ProjectTo<TDestination>(map);
        }


        #endregion

        public static TDestination MapTo<TDestination>(this object source)
        {
            return GetConfiguration(source.GetType(), typeof(TDestination)).CreateMapper().Map<TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source,
            Action<IMappingExpression<TSource, TDestination>> destinationAction = null,
            Action<IMappingExpression<TDestination, TSource>> sourceAction = null,
            Action<IMapperConfigurationExpression> configAction = null)
            where TSource : class
        {
            var map = new MapperConfiguration(cfg =>
            {
                var destinationExpression = cfg.CreateMap<TSource, TDestination>();
                var sourceExpression = cfg.CreateMap<TDestination, TSource>();
                destinationAction?.Invoke(destinationExpression);
                sourceAction?.Invoke(sourceExpression);
                configAction?.Invoke(cfg);
            });

            return map.CreateMapper().Map<TSource, TDestination>(source);
        }

        public static IMappingExpression<TSource, TDestination> ToMember<TSource, TDestination, TDestinonResult, TSourceResult>(this IMappingExpression<TSource, TDestination> expression, Expression<Func<TDestination, TDestinonResult>> destinationFunc, Expression<Func<TSource, TSourceResult>> sourcefunc)
        {
            return expression.ForMember(destinationFunc, conf => conf.MapFrom(sourcefunc));
        }

        public static IMappingExpression<TSource, TDestination> IgnoreMember<TSource, TDestination, TResult>(this IMappingExpression<TSource, TDestination> expression, Expression<Func<TDestination, TResult>> destinationFunc)
        {
            return expression.ForMember(destinationFunc, conf => conf.Ignore());
        }
    }

    public sealed class MapType
    {
        public Type SourceType { get; }

        public Type DestinationType { get; }

        public MapType(Type sourceType, Type destinationType)
        {
            SourceType = sourceType;
            DestinationType = destinationType;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() == GetType() == false)
            {
                return false;
            }
            MapType temp = null;
            temp = (MapType)obj;

            return SourceType == temp.SourceType && DestinationType == temp.DestinationType;
        }

        public override int GetHashCode()
        {
            return (SourceType?.GetHashCode() + DestinationType?.GetHashCode()).GetValueOrDefault();
        }
    }

}

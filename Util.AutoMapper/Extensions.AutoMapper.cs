using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace Util.AutoMapper
{
    public static partial class AutoMapperExtensions
    {
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
            var map = new MapperConfiguration(cfg =>
            {
                var sourceType = query.GetType().GetGenericArguments()[0];
                var destinationType = typeof(TDestination);
                cfg.CreateMap(sourceType, destinationType);
                cfg.CreateMap(destinationType, sourceType);
            });

            return query.ProjectTo<TDestination>(map);
        }
        #endregion

        public static IMappingExpression<TSource, TDestination> ToMember<TSource, TDestination, TDestinonResult, TSourceResult>(this IMappingExpression<TSource, TDestination> expression, Expression<Func<TDestination, TDestinonResult>> destinationFunc, Expression<Func<TSource, TSourceResult>> sourcefunc)
        {
            return expression.ForMember(destinationFunc, conf => conf.MapFrom(sourcefunc));
        }

        public static IMappingExpression<TSource, TDestination> IgnoreMember<TSource, TDestination, TResult>(this IMappingExpression<TSource, TDestination> expression, Expression<Func<TDestination, TResult>> destinationFunc)
        {
            return expression.ForMember(destinationFunc, conf => conf.Ignore());
        }
    }
}

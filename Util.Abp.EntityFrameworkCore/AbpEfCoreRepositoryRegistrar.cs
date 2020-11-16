using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.Domain.Entities;
using Volo.Abp.EntityFrameworkCore.DependencyInjection;
using Volo.Abp.Reflection;

namespace Volo.Abp.EntityFrameworkCore
{
    public class AbpEfCoreRepositoryRegistrar : EfCoreRepositoryRegistrar
    {
        public static IEnumerable<Type> ModelTypes { get; set; }

        public AbpEfCoreRepositoryRegistrar(AbpDbContextRegistrationOptions options) : base(options)
        {
        }

        protected override IEnumerable<Type> GetEntityTypes(Type dbContextType)
        {
            var infos =
                (from property in dbContextType.GetTypeInfo().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                 where
                     ReflectionHelper.IsAssignableToGenericType(property.PropertyType, typeof(DbSet<>)) &&
                     typeof(IEntity).IsAssignableFrom(property.PropertyType.GenericTypeArguments[0])
                 select property.PropertyType.GenericTypeArguments[0]).ToList();

            var assembly = ModelTypes;

            if (assembly != null)
            {
                infos.AddRange(from type in assembly where type.IsClass && IsAssignableToGenericType(type, typeof(IEntity<>)) select type);
            }

            return infos;
        }

        private static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var givenTypeInfo = givenType.GetTypeInfo();

            if (givenTypeInfo.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            foreach (var interfaceType in givenType.GetInterfaces())
            {
                if (interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }

            if (givenTypeInfo.BaseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(givenTypeInfo.BaseType, genericType);
        }
    }

}

using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Linq.Expressions;
using AccountManager.Common.Extensions;

namespace AccountManager.Persistence.Configurations
{
    public abstract class EntityTypeConfigurationBase<T> : EntityTypeConfiguration<T> where T : class
    {
        private static HashSet<Type> MappableTypes = new HashSet<Type>()
        {
            typeof(string), typeof(bool), typeof(DateTime), typeof(DateTimeOffset), typeof(byte[])
        };

        protected virtual IDictionary<string, string> ColumnMappings => new Dictionary<string, string>();

        protected void MapProperies()
        {
            foreach (var property in typeof(T).GetProperties()
                .Where(p => ShouldMap(p.PropertyType)))
            {
                var parameter = Expression.Parameter(typeof(T), "e");
                var member = Expression.Property(parameter, property.Name);

                string columnName;

                if (!ColumnMappings.TryGetValue(property.Name, out columnName))
                {
                    columnName = property.Name.ToUnderscoreCase();
                }

                var lambdaExpression = Expression.Lambda(member, parameter);
                if (columnName == null)
                {
                    Ignore((dynamic)lambdaExpression);
                }
                else
                {
                    Property((dynamic)lambdaExpression).HasColumnName(columnName);
                }
            }
        }

        private bool ShouldMap(Type type)
        {
            var typeToCheck = Nullable.GetUnderlyingType(type) ?? type;
            return typeToCheck.IsPrimitive ||
                   typeToCheck.IsEnum ||
                   MappableTypes.Contains(typeToCheck);
        }
    }
}
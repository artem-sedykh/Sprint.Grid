using Sprint.Linq;

namespace Sprint.Grid.Impl
{
    using Dynamic;    
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.Helpers;
    using System;
    using System.Linq;    
    using System.Linq.Expressions;
    using System.Web.Mvc;

    public static class GridQueryableExtensions
    {        
        internal static IQueryable<IGroupingItem> GroupBy<TModel>(this IQueryable<TModel> source, IDictionary<LambdaExpression, SortDirection?> properties)
        {
            var groupFields = properties.Select(x => x.Key).ToArray();
            var directions = properties.Select(x => x.Value).ToArray();
            var length = groupFields.Length;
           
            var dynamicProperies = new List<DynamicProperty>();

            var keyParameter = Expression.Parameter(source.ElementType, "key");
            var keyByDirection = new Dictionary<LambdaExpression, SortDirection?>();
            var memberBindings = new MemberBinding[length];

            for (var i = 0; i < length; i++)
            {
                var key = String.Format("Key{0}", i);
                dynamicProperies.Add(new DynamicProperty(key, groupFields[i].ReturnType));                
            }

            var keyType = ClassFactory.Instance.GetDynamicClass(dynamicProperies);            

            var groupType = typeof (IGrouping<,>).MakeGenericType(keyType, typeof (TModel));
            var sortParameter = Expression.Parameter(groupType, "p");
            var keyGroupProperty = Expression.Property(sortParameter, "Key"); 
           
            for (var i = 0; i < length; i++)
            {
                var key = String.Format("Key{0}", i);
                var property = Expression.Lambda(Expression.Property(keyGroupProperty, key), sortParameter);
                var pKey = Expression.Invoke(groupFields[i], keyParameter);
                keyByDirection[property] = directions[i];
                memberBindings[i] = Expression.Bind(keyType.GetMember(key)[0], pKey);
            }
            
            var keySelector = Expression.Lambda(Expression.MemberInit(Expression.New(keyType), memberBindings), new[] { keyParameter }).Expand() as LambdaExpression;       

            var query = source.CallQueryableMethod("GroupBy", keySelector).Sort(keyByDirection) as IQueryable<IGrouping<object,TModel>>;

            return query != null ? query.Select(x => new GroupingItem {Count = x.Count(), Key = x.Key}) : null;
        }        
         
        internal static IQueryable<TModel> Where<TModel>(this IQueryable<TModel> source, LambdaExpression property, object rawValue)
        {
            var parameter = Expression.Parameter(source.ElementType, "p");
            var currentProperty = Expression.Invoke(property, parameter);
            var valueProviderResult = new ValueProviderResult(rawValue, rawValue != null ? rawValue.ToString() : null, CultureInfo.InvariantCulture);
            var value = valueProviderResult.ConvertTo(property.ReturnType);
            var constant = Expression.Constant(value, property.ReturnType);
            var expression = Expression.Equal(currentProperty, constant);
            var lambda = Expression.Lambda<Func<TModel, bool>>(expression, parameter).Expand();

            return source.Where(lambda);
        }

        internal static IQueryable<TModel> Sort<TModel>(this IQueryable<TModel> query, IDictionary<LambdaExpression, SortDirection?> propertyByDirection)
        {
            return (query as IQueryable).Sort(propertyByDirection) as IQueryable<TModel>;
        }

        internal static IQueryable Sort(this IQueryable query, IDictionary<LambdaExpression, SortDirection?> propertyByDirection)
        {            
            if (propertyByDirection.Count == 0)
                return query;
            
            var first = true;

            foreach (var item in propertyByDirection)
            {
                var property = item.Key;
                var direction = item.Value;

                if (first)
                {
                    if (direction == SortDirection.Ascending)
                        query = query.CallQueryableMethod("OrderBy", property);

                    if (direction == SortDirection.Descending)
                        query = query.CallQueryableMethod("OrderByDescending", property);

                    first = false;
                }
                else
                {
                    if (direction == SortDirection.Ascending)
                        query = query.CallQueryableMethod("ThenBy", property);

                    if (direction == SortDirection.Descending)
                        query = query.CallQueryableMethod("ThenByDescending", property);
                }

            }

            return query;
        }                        

        private static IQueryable CallQueryableMethod(this IQueryable source, string methodName, LambdaExpression selector)
        {
            var query = source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    methodName,
                    new[] { source.ElementType, selector.Body.Type },
                    source.Expression,
                    Expression.Quote(selector)));
            
            return query;
        }       
    }    
}

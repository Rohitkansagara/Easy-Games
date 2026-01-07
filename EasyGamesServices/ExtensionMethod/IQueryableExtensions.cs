using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EasyGames.Services.ExtensionMethod
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> query, string propertyName, bool ascending)
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var selector = Expression.PropertyOrField(parameter, propertyName);
            var lambda = Expression.Lambda(selector, parameter);
            string methodName = ascending ? "OrderBy" : "OrderByDescending";
            var result = Expression.Call(typeof(Queryable), methodName,
                new Type[] { typeof(T), selector.Type },
                query.Expression, Expression.Quote(lambda));
            return query.Provider.CreateQuery<T>(result);
        }
    }

}

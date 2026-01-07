using EasyGames.Class.DATA;
using EasyGames.Services.ExtensionMethod;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace EasyGames.Services.Services
{
    public interface IGenericFilterService
    {
        Task<PagedResult<TResult>> GetFilteredDataAsync<TEntity, TResult>(
            IQueryable<TEntity> query,
            int pageNo,
            int pageSize,
            string? filter,
            string? orderBy,
            Expression<Func<TEntity, TResult>> selector,
            IDictionary<string, string>? defaultFilters = null,
            bool throwOnInvalidColumn = false
        )
        where TEntity : class;
    }


    /// <summary>
    /// Generic dynamic filter / sort / paginate service supporting:
    /// - AND / OR
    /// - eq, neq, gt, lt, gte, lte
    /// - contains, startsWith, endsWith
    /// - in (pipe-separated)
    /// - date range (type "5" with "~")
    /// - numeric between (operator "between" with "~")
    /// - null / notnull
    /// - automatic case-insensitive column matching
    /// - optional default filters and column validation
    /// </summary>
    public class GenericFilterService : IGenericFilterService
    {
        public async Task<PagedResult<TResult>> GetFilteredDataAsync<TEntity, TResult>(
            IQueryable<TEntity> query,
            int pageNo,
            int pageSize,
            string? filter,
            string? orderBy,
            Expression<Func<TEntity, TResult>> selector,
            IDictionary<string, string>? defaultFilters = null,
            bool throwOnInvalidColumn = false)
            where TEntity : class
        {
            // Normalize incoming filter string
            filter = string.IsNullOrWhiteSpace(filter) ? string.Empty : filter.Trim();

            // 1) Build a list of filter clauses from the 'filter' query string
            // We'll support AND sections separated by ",and," where each AND section may contain ",or," parts.
            // Example: "status,eq,Approved,or,status,eq,Pending,and,propertyid,eq,12"
            var providedColumns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (!string.IsNullOrEmpty(filter)) //filter = "status,eq,Approved,or,status,eq,Pending,and,entrydate,5,2025-11-01~2025-11-20"
            {
                var andParts = filter.Split(",and,", StringSplitOptions.RemoveEmptyEntries);

                foreach (var andPart in andParts) //status,eq,Approved,or,status,eq,Pending
                {
                    // orParts will be combined with OR
                    Expression<Func<TEntity, bool>>? orGroup = null;

                    var orParts = andPart.Split(",or,", StringSplitOptions.RemoveEmptyEntries);

                    foreach (var orPart in orParts)//status,eq,Approved

                    {
                        var tokens = orPart.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()).ToArray(); //status eq Approved
                        if (tokens.Length < 2)
                            continue;

                        // Support two forms:
                        // 1) column,op,value
                        // 2) column,op  (for op = null / notnull)
                        string columnToken = tokens[0];
                        string op = tokens.Length >= 2 ? tokens[1].ToLower() : string.Empty;
                        string value = tokens.Length >= 3 ? tokens[2] : string.Empty;

                        // Map columnToken (case-insensitive) to actual property name
                        var mapped = GetPropertyNameCaseInsensitive<TEntity>(columnToken);
                        if (mapped == null)
                        {
                            if (throwOnInvalidColumn)
                                throw new ArgumentException($"Invalid column '{columnToken}'. Valid: {string.Join(", ", GetEntityPropertyNames<TEntity>())}");
                            // skip this clause
                            continue;
                        }

                        providedColumns.Add(mapped);

                        var param = Expression.Parameter(typeof(TEntity), "x");
                        var propertyExpr = Expression.PropertyOrField(param, mapped);

                        var conditionBody = BuildCondition(propertyExpr as MemberExpression, op, value);
                        if (conditionBody == null)
                            continue;

                        var predicate = Expression.Lambda<Func<TEntity, bool>>(conditionBody, param);

                        // accumulate ORs
                        orGroup = orGroup == null ? predicate : OrAlso(orGroup, predicate);
                    }

                    // apply this AND group
                    if (orGroup != null)
                    {
                        query = query.Where(orGroup);
                    }
                }
            }

            // 2) Apply default filters (if provided) for columns not present in providedColumns
            if (defaultFilters != null && defaultFilters.Count > 0)
            {
                foreach (var kv in defaultFilters)
                {
                    var colToken = kv.Key;
                    var defaultVal = kv.Value;

                    var mapped = GetPropertyNameCaseInsensitive<TEntity>(colToken);
                    if (mapped == null)
                    {
                        if (throwOnInvalidColumn)
                            throw new ArgumentException($"Invalid default filter column '{colToken}'. Valid: {string.Join(", ", GetEntityPropertyNames<TEntity>())}");
                        continue;
                    }

                    if (providedColumns.Contains(mapped))
                        continue; // user-specified filter already exists; don't override

                    // default filter format supports same operators; we will expect "op:value" or just "value" which maps to eq
                    // Acceptable defaultFormats:
                    // "eq:244" or "244" (implies eq)
                    // "in:1|2|3"
                    // "5:2025-11-01~2025-11-10"
                    // "between:10~20"
                    // "null" / "notnull"
                    string defOp, defValue;
                    if (defaultVal.Contains(':'))
                    {
                        var parts = defaultVal.Split(':', 2);
                        defOp = parts[0].Trim().ToLower();
                        defValue = parts[1].Trim();
                    }
                    else
                    {
                        defOp = "eq";
                        defValue = defaultVal;
                    }

                    var param = Expression.Parameter(typeof(TEntity), "x");
                    var propertyExpr = Expression.PropertyOrField(param, mapped);

                    var body = BuildCondition(propertyExpr as MemberExpression, defOp, defValue);
                    if (body == null) continue;

                    var lambda = Expression.Lambda<Func<TEntity, bool>>(body, param);
                    query = query.Where(lambda);
                }
            }

            // 3) Apply ordering
            if (!string.IsNullOrEmpty(orderBy))
            {
                var orderParts = orderBy.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var orderColToken = orderParts[0];
                var mappedOrderCol = GetPropertyNameCaseInsensitive<TEntity>(orderColToken);
                if (mappedOrderCol == null)
                {
                    if (throwOnInvalidColumn)
                        throw new ArgumentException($"Invalid orderBy column '{orderColToken}'. Valid: {string.Join(", ", GetEntityPropertyNames<TEntity>())}");
                    // else ignore ordering
                }
                else
                {
                    bool asc = orderParts.Length < 2 || orderParts[1].Equals("asc", StringComparison.OrdinalIgnoreCase);
                    query = asc ? query.OrderByDynamic(mappedOrderCol, true) : query.OrderByDynamic(mappedOrderCol, false);
                }
            }

            // 4) Pagination & projection
            int totalCount = await query.CountAsync();

            var data = await query
                .Skip(Math.Max(0, (pageNo - 1)) * pageSize)
                .Take(pageSize)
                .Select(selector)
                .ToListAsync();

            return new PagedResult<TResult>
            {
                Data = data,
                TotalCount = totalCount,
                PageNo = pageNo,
                PageSize = pageSize
            };
        }

        // -------------------------
        // Helpers
        // -------------------------

        /// <summary>
        /// Build a single condition Expression for a given property, operator and value.
        /// property may be a MemberExpression representing x.Property.
        /// Returns null when the operator/value cannot be parsed.
        /// </summary>
        private Expression? BuildCondition(MemberExpression? property, string op, string value)
        {
            if (property == null)
                return null;

            // handle null / notnull operators
            if (op == "null")
            {
                // x.Property == null
                return Expression.Equal(property, Expression.Constant(null, property.Type));
            }
            if (op == "notnull")
            {
                return Expression.NotEqual(property, Expression.Constant(null, property.Type));
            }

            // special-case date range type "5": value format "start~end"
            if (op == "5")
            {
                if (string.IsNullOrEmpty(value) || !value.Contains('~')) return null;
                var parts = value.Split('~', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2) return null;
                if (!DateTime.TryParse(parts[0], out var start)) return null;
                if (!DateTime.TryParse(parts[1], out var end)) return null;

                var lower = Expression.GreaterThanOrEqual(property, Expression.Constant(start));
                var upper = Expression.LessThanOrEqual(property, Expression.Constant(end));
                return Expression.AndAlso(lower, upper);
            }

            // numeric between: operator "between" value "min~max"
            if (op == "between")
            {
                if (!value.Contains('~')) return null;
                var parts = value.Split('~', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 2) return null;

                // support numeric types
                var underlyingType = Nullable.GetUnderlyingType(property.Type) ?? property.Type;

                if (underlyingType == typeof(int) && int.TryParse(parts[0], out var minI) && int.TryParse(parts[1], out var maxI))
                {
                    var lower = Expression.GreaterThanOrEqual(property, Expression.Constant(minI));
                    var upper = Expression.LessThanOrEqual(property, Expression.Constant(maxI));
                    return Expression.AndAlso(lower, upper);
                }
                if (underlyingType == typeof(long) && long.TryParse(parts[0], out var minL) && long.TryParse(parts[1], out var maxL))
                {
                    var lower = Expression.GreaterThanOrEqual(property, Expression.Constant(minL));
                    var upper = Expression.LessThanOrEqual(property, Expression.Constant(maxL));
                    return Expression.AndAlso(lower, upper);
                }
                if (underlyingType == typeof(decimal) && decimal.TryParse(parts[0], out var minD) && decimal.TryParse(parts[1], out var maxD))
                {
                    var lower = Expression.GreaterThanOrEqual(property, Expression.Constant(minD));
                    var upper = Expression.LessThanOrEqual(property, Expression.Constant(maxD));
                    return Expression.AndAlso(lower, upper);
                }

                return null;
            }

            // IN operator: value is pipe-separated: "1|2|3" or "a|b|c"
            if (op == "in")
            {
                if (string.IsNullOrEmpty(value)) return null;
                var items = value.Split('|', StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToArray();
                var underlyingType = Nullable.GetUnderlyingType(property.Type) ?? property.Type;

                if (underlyingType == typeof(int))
                {
                    var ints = items.Where(s => int.TryParse(s, out _)).Select(int.Parse).ToList();
                    if (!ints.Any()) return null;
                    var listConst = Expression.Constant(ints);
                    var contains = typeof(List<int>).GetMethod("Contains", new[] { typeof(int) })!;
                    return Expression.Call(listConst, contains, Expression.Convert(property, typeof(int)));
                }
                if (underlyingType == typeof(long))
                {
                    var longs = items.Where(s => long.TryParse(s, out _)).Select(long.Parse).ToList();
                    if (!longs.Any()) return null;
                    var listConst = Expression.Constant(longs);
                    var contains = typeof(List<long>).GetMethod("Contains", new[] { typeof(long) })!;
                    return Expression.Call(listConst, contains, Expression.Convert(property, typeof(long)));
                }
                if (underlyingType == typeof(string))
                {
                    var listConst = Expression.Constant(items.ToList());
                    var contains = typeof(List<string>).GetMethod("Contains", new[] { typeof(string) })!;
                    return Expression.Call(listConst, contains, property);
                }

                return null;
            }

            // For the rest, create a typed constant according to property type
            var propUnderlying = Nullable.GetUnderlyingType(property.Type) ?? property.Type;

            Expression? constantExpr = null;
            if (propUnderlying == typeof(string))
            {
                constantExpr = Expression.Constant(value, typeof(string));
            }
            else if (propUnderlying == typeof(DateTime))
            {
                if (!DateTime.TryParse(value, out var dt)) return null;
                constantExpr = Expression.Constant(dt, typeof(DateTime));
            }
            else if (propUnderlying == typeof(int))
            {
                if (!int.TryParse(value, out var iVal)) return null;
                constantExpr = Expression.Constant(iVal, typeof(int));
            }
            else if (propUnderlying == typeof(long))
            {
                if (!long.TryParse(value, out var lVal)) return null;
                constantExpr = Expression.Constant(lVal, typeof(long));
            }
            else if (propUnderlying == typeof(decimal))
            {
                if (!decimal.TryParse(value, out var dec)) return null;
                constantExpr = Expression.Constant(dec, typeof(decimal));
            }
            else if (propUnderlying == typeof(bool))
            {
                if (!bool.TryParse(value, out var bv)) return null;
                constantExpr = Expression.Constant(bv, typeof(bool));
            }
            else
            {
                // fallback to string comparison for unknown types
                constantExpr = Expression.Constant(value, typeof(string));
            }

            if (constantExpr == null) return null;

            // Map simple operators and string functions
            switch (op)
            {
                case "eq":
                    return Expression.Equal(property, Expression.Convert(constantExpr, property.Type));
                case "neq":
                    return Expression.NotEqual(property, Expression.Convert(constantExpr, property.Type));
                case "gt":
                    return Expression.GreaterThan(property, Expression.Convert(constantExpr, property.Type));
                case "lt":
                    return Expression.LessThan(property, Expression.Convert(constantExpr, property.Type));
                case "gte":
                    return Expression.GreaterThanOrEqual(property, Expression.Convert(constantExpr, property.Type));
                case "lte":
                    return Expression.LessThanOrEqual(property, Expression.Convert(constantExpr, property.Type));
                case "contains" when propUnderlying == typeof(string):
                    return Expression.Call(property, typeof(string).GetMethod("Contains", new[] { typeof(string) })!, constantExpr);
                case "startswith" when propUnderlying == typeof(string):
                    return Expression.Call(property, typeof(string).GetMethod("StartsWith", new[] { typeof(string) })!, constantExpr);
                case "endswith" when propUnderlying == typeof(string):
                    return Expression.Call(property, typeof(string).GetMethod("EndsWith", new[] { typeof(string) })!, constantExpr);
                default:
                    return null;
            }
        }

        // Build OR combination of two predicates (left OR right)
        private Expression<Func<T, bool>> OrAlso<T>(Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var param = Expression.Parameter(typeof(T), "x");
            var leftBody = new ReplaceExpressionVisitor(left.Parameters[0], param).Visit(left.Body)!;
            var rightBody = new ReplaceExpressionVisitor(right.Parameters[0], param).Visit(right.Body)!;
            return Expression.Lambda<Func<T, bool>>(Expression.OrElse(leftBody, rightBody), param);
        }

        // Map incoming column token (case-insensitive) to actual CLR property name.
        // Returns null if not found.
        private static string? GetPropertyNameCaseInsensitive<TEntity>(string columnToken)
        {
            if (string.IsNullOrWhiteSpace(columnToken)) return null;
            var props = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var found = props.FirstOrDefault(p => string.Equals(p.Name, columnToken, StringComparison.OrdinalIgnoreCase));
            return found?.Name;
        }

        // List of CLR property names for an entity type, used in validation errors
        private static IEnumerable<string> GetEntityPropertyNames<TEntity>()
        {
            return typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => p.Name);
        }
    }


    // ReplaceExpressionVisitor (used to rewrite parameter references when combining lambdas)
    public class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldExp;
        private readonly Expression _newExp;

        public ReplaceExpressionVisitor(Expression oldExp, Expression newExp)
        {
            _oldExp = oldExp;
            _newExp = newExp;
        }

        public override Expression? Visit(Expression? node)
        {
            if (node == _oldExp) return _newExp;
            return base.Visit(node);
        }
    }
}

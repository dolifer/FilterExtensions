using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using FilterExtensions.Constants;
using FilterExtensions.Filters;

namespace FilterExtensions
{
    public static class QueryableExtensions
    {
        private static readonly Dictionary<string, string> _operators = new()
        {
            {Operators.Equal, "="},
            {Operators.NotEqual, "!="},
            {Operators.GreaterThan, ">"},
            {Operators.GreaterThanOrEqual, ">="},
            {Operators.LessThan, "<"},
            {Operators.LessThanOrEqual, "<="}
        };

        public static IQueryable<T> Where<T>(this IQueryable<T> source, Filter filter)
        {
            if (source is null || filter is null)
            {
                return source;
            }

            return source.ApplyFilter(filter);
        }

        public static string BuildPredicate(Filter filter)
        {
            var (predicate, _) = ParseFilter(filter);
            return predicate;
        }

        private static (string predicate, object[] values) ParseFilter(Filter filter)
        {
            EnsureFilterIsValid(filter);

            var nestedFilters = GetNestedFilters(filter);
            var values = nestedFilters.Select(f => f.Value).ToArray();

            var predicate = BuildPredicate(filter, nestedFilters, true);

            return (predicate, values);
        }

        private static void EnsureFilterIsValid(Filter filter)
        {
            if (filter is null)
            {
                throw new ArgumentException("Filter can not be null.", nameof(filter));
            }

            if (filter.Filters != null && filter.Filters.Any())
            {
                if (string.IsNullOrWhiteSpace(filter.Logic))
                {
                    throw new ArgumentException("Logic property is required.", nameof(Filter.Logic));
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(filter.Operator))
                {
                    throw new ArgumentException("Operator property is required.", nameof(Filter.Operator));
                }
            }
        }

        private static IQueryable<T> ApplyFilter<T>(this IQueryable<T> source, Filter filter)
        {
            var (predicate, values) = ParseFilter(filter);
            return source.Where(predicate, values);
        }

        private static IList<Filter> GetNestedFilters(Filter filter)
        {
            var filters = new List<Filter>();
            GetFilters(filter, filters);
            return filters;

            static void GetFilters(Filter filter, ICollection<Filter> filters)
            {
                if (filter.Filters != null && filter.Filters.Any())
                {
                    if (!string.IsNullOrWhiteSpace(filter.Operator))
                    {
                        filters.Add(filter);
                    }

                    foreach (var item in filter.Filters)
                    {
                        GetFilters(item, filters);
                    }
                }
                else
                {
                    filters.Add(filter);
                }
            }
        }

        private static string BuildPredicate(Filter filter, IList<Filter> nestedFilters, bool isRoot)
        {
            var prefix = string.IsNullOrWhiteSpace(filter.Property) && isRoot ? "x => " : "";

            string GetTopLevelQuery()
            {
                var index = nestedFilters.IndexOf(filter);
                var comparison = _operators[filter.Operator];

                var field = string.IsNullOrWhiteSpace(filter.Property) ? "x" : filter.Property;

                return $"{field} {comparison} @{index}";
            }

            if (filter.Filters == null || !filter.Filters.Any())
            {
                var top = GetTopLevelQuery();
                return $"{prefix}{top}";
            }

            var nested = filter.Filters.Select(f => BuildPredicate(f, nestedFilters, false));
            var separator = " " + filter.Logic + " ";

            if (string.IsNullOrWhiteSpace(filter.Operator))
            {
                return "(" + string.Join(separator, nested.ToArray()) + ")";
            }

            var topLevel = GetTopLevelQuery();
            return $"{prefix}(" + topLevel + " " + filter.Logic + " " + string.Join(separator, nested.ToArray()) + ")";
        }
    }
}
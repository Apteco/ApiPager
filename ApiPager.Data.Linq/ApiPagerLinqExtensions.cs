using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Linq.Dynamic.Core;
using ApiPager.Core;
using ApiPager.Core.FilterExpression;

namespace ApiPager.Data.Linq
{
  public static class ApiPagerLinqExtensions
  {
    #region public methods
    public static IEnumerable<T> Filter<T>(this IEnumerable<T> items, FilterPageAndSortInfo filterPageAndSortInfo, string[] filterColumns, string candidateKeyColumn)
    {
      return Filter(items, filterPageAndSortInfo, filterColumns, new string[] { candidateKeyColumn });
    }

    public static IEnumerable<T> Filter<T>(this IEnumerable<T> items, FilterPageAndSortInfo filterPageAndSortInfo, string[] filterColumns, string[] candidateKeyColumns)
    {
      if (filterPageAndSortInfo == null)
        return items;

      IQueryable<T> filteredQueryable;
      if (string.IsNullOrEmpty(filterPageAndSortInfo.Filter))
      {
        filteredQueryable = items.AsQueryable();
      }
      else
      {
        filteredQueryable = items
          .AsQueryable()
          .Where(CreateDynamicLinqFilterClause(filterPageAndSortInfo, filterColumns));
      }

      IQueryable<T> filteredSortedQueryable = filteredQueryable
        .OrderBy(CreateOrderByClause(filterPageAndSortInfo, candidateKeyColumns));

      filterPageAndSortInfo.TotalCount = filteredSortedQueryable.Count();

      List<T> processedItems = filteredSortedQueryable
        .Skip(filterPageAndSortInfo.Offset)
        .Take(filterPageAndSortInfo.Count)
        .ToList();

      return processedItems;
    }
    #endregion

    #region private methods
    private static string CreateOrderByClause(FilterPageAndSortInfo filterPageAndSortInfo, string[] candidateKeyColumns)
    {
      if (string.IsNullOrEmpty(filterPageAndSortInfo.OrderByColumn) && (candidateKeyColumns.Length == 0))
        throw new ArgumentException("Must specify either an OrderByColumn in the pageAndSortInfo or at least one candidateKeyColumn");

      StringBuilder orderByClause = new StringBuilder();
      if (!string.IsNullOrEmpty(filterPageAndSortInfo.OrderByColumn))
      {
        orderByClause.Append(filterPageAndSortInfo.OrderByColumn);
        orderByClause.Append(" ").Append(filterPageAndSortInfo.Order == SortOrderEnum.Ascending ? "ASC" : "DESC").Append(", ");
      }

      orderByClause.Append(string.Join(", ", candidateKeyColumns));
      return orderByClause.ToString();
    }

    private static string CreateDynamicLinqFilterClause(FilterPageAndSortInfo filterPageAndSortInfo, string[] filterColumns)
    {
      if (string.IsNullOrEmpty(filterPageAndSortInfo.Filter))
        return "";

      StringBuilder builder = new StringBuilder();

      Expression expression = ExpressionParser.ParseExpression(filterPageAndSortInfo.Filter);
      string errorMessages;
      if (!expression.AreFieldsValid(filterColumns, out errorMessages))
        throw new Exception("The filter expression is not valid: " + errorMessages);

      builder.Append(new FilterExpressionLinqBuilder(filterColumns).CreateFilterExpression(expression));
      return builder.ToString();
    }
    #endregion
  }
}

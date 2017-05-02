using System;
using ApiPager.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace ApiPager.AspNetCore
{
  public static class ApiPagerUtilities
  {
    #region private constants
    private const int DefaultOffset = 0;
    private const int DefaultCount = 10;
    #endregion

    #region public methods
    public static FilterPageAndSortInfo GetFilterPageAndSortInfo(this HttpContext httpContext, int defaultOffset = DefaultOffset, int defaultCount = DefaultCount)
    {
      string orderBy = httpContext?.Request?.Query?["orderBy"];
      SortOrderEnum order = SortOrderEnum.Ascending;

      if (!string.IsNullOrEmpty(orderBy) && orderBy.StartsWith("-"))
      {
        orderBy = orderBy.Substring(1);
        order = SortOrderEnum.Descending;
      }
 
      return new FilterPageAndSortInfo()
      {
        Filter = httpContext?.Request?.Query?["filter"],
        Offset = GetInt(httpContext?.Request?.Query?["offset"], defaultOffset),
        Count = GetInt(httpContext?.Request?.Query?["count"], defaultCount),
        OrderByColumn = orderBy,
        Order = order 
      };
    }
    #endregion

    #region private methods
    private static int GetInt(StringValues? values, int defaultValue)
    {
      int intValue;
      if (!int.TryParse(values, out intValue))
        return defaultValue;
      
      return intValue;
    }
    #endregion
  }
}

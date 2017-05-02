using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiPager.Core.Models
{
  /// <summary>
  /// Summary details for a user
  /// </summary>
  public class PagedResults<T> : IPagedResults
  {
    #region private fields
    private FilterPageAndSortInfo filterPageAndSortInfo;
    #endregion

    #region public properties
    /// <summary>
    /// The number of items that were skipped over from the (potentially filtered) result set
    /// </summary>
    [Required]
    public int? Offset
    {
      get { return filterPageAndSortInfo?.Offset; }
      set
      {
        if (value == null)
          return;

        if (filterPageAndSortInfo == null)
          filterPageAndSortInfo = new FilterPageAndSortInfo();

        filterPageAndSortInfo.Offset = value.Value;
      }
    }

    /// <summary>
    /// The number of items returned in this page of the result set
    /// </summary>
    [Required]
    public int? Count
    {
      get { return ApiPagerUtilities.Min(filterPageAndSortInfo?.Count, filterPageAndSortInfo?.TotalCount); }
      set
      {
        if (value == null)
          return;

        if (filterPageAndSortInfo == null)
          filterPageAndSortInfo = new FilterPageAndSortInfo();

        filterPageAndSortInfo.Count = value.Value;
      }
    }

    /// <summary>
    /// The total number of items available in the (potentially filtered) result set
    /// </summary>
    [Required]
    public int? TotalCount
    {
      get { return filterPageAndSortInfo?.TotalCount; }
      set
      {
        if (value == null)
          return;

        if (filterPageAndSortInfo == null)
          filterPageAndSortInfo = new FilterPageAndSortInfo();

        filterPageAndSortInfo.TotalCount = value.Value;
      }
    }

    /// <summary>
    /// The list of results
    /// </summary>
    [Required]
    public List<T> List { get; set; }
    #endregion

    #region public methods
    public void SetFilterPageAndSortInfo(FilterPageAndSortInfo filterPageAndSortInfo)
    {
      this.filterPageAndSortInfo = filterPageAndSortInfo;
    }

    public FilterPageAndSortInfo GetFilterPageAndSortInfo()
    {
      return filterPageAndSortInfo;
    }
    #endregion
  }
}

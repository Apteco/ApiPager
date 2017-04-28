namespace ApiPager.Core
{
  public class FilterPageAndSortInfo
  {
    #region public properties
    public int Offset { get; set; }
    public int Count { get; set; }
    public string OrderByColumn { get; set; }
    public SortOrderEnum Order { get; set; }
    public string Filter { get; set; }
    public int? TotalCount { get; set; }
    #endregion

    #region public constructor
    public FilterPageAndSortInfo()
    {
      Offset = 0;
      Count = 10;
    }
    #endregion
  }
}

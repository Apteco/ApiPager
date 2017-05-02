using ApiPager.Core;

namespace ApiPager.Core.Models
{
  public interface IPagedResults
  {
    void SetFilterPageAndSortInfo(FilterPageAndSortInfo filterPageAndSortInfo);
    FilterPageAndSortInfo GetFilterPageAndSortInfo();
  }
}

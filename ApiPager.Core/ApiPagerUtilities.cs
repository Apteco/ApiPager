using System;

namespace ApiPager.Core
{
  public static class ApiPagerUtilities
  {
    #region public methods
    public static int? Min(int? x, int? y)
    {
      if ((x == null) && (y == null))
      {
        return null;
      }
      else if ((x == null) && (y != null))
      {
        return y;
      }
      else if ((x != null) && (y == null))
      {
        return x;
      }
      else
      {
        return Math.Min(x.Value, y.Value);
      }
    }
    #endregion
  }
}

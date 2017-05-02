using System;

namespace ApiPager.Core
{
  [AttributeUsage(AttributeTargets.Method, Inherited = false)]
  public class CanFilterPageAndSortAttribute : Attribute
  {
    #region public properties
    public string[] AvailableFields { get; set; }
    #endregion

    #region public constructor
    public CanFilterPageAndSortAttribute()
      : this (new string[0])
    {
    }

    public CanFilterPageAndSortAttribute(string[] availableFields)
    {
      AvailableFields = availableFields;
    }
    #endregion
  }
}

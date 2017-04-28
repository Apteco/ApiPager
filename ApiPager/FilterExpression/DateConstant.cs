using System;
using System.Globalization;

namespace ApiPager.Core.FilterExpression
{
  public class DateConstant : AbstractConstant
  {
    #region public properties
    public DateTime Value { get; set; }
    #endregion

    #region public methods
    public override string ToString()
    {
      CultureInfo britishEnglish = new CultureInfo("en-GB");

      if (Value.TimeOfDay == new TimeSpan(0,0,0,0))
        return "@" + Value.ToString("d", britishEnglish);
      else
        return "@" + Value.ToString("d", britishEnglish) + "_" + Value.ToString("T", britishEnglish);
    }
    #endregion
  }
}

namespace ApiPager.Core.FilterExpression
{
  public class StringConstant : AbstractConstant
  {
    #region public properties
    public string Value { get; set; }
    #endregion

    #region public methods
    public override string ToString()
    {
      return "\""+Value+"\"";
    }
    #endregion
  }
}

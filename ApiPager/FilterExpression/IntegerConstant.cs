namespace ApiPager.Core.FilterExpression
{
  public class IntegerConstant : AbstractConstant
  {
    #region public properties
    public int Value { get; set; }
    #endregion

    #region public methods
    public override string ToString()
    {
      return "#" + Value;
    }
    #endregion
  }
}

namespace ApiPager.Core.FilterExpression
{
  public class RealConstant : AbstractConstant
  {
    #region public properties
    public double Value { get; set; }
    #endregion

    #region public methods
    public override string ToString()
    {
      return "d#" + Value;
    }
    #endregion
  }
}

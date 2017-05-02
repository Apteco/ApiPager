using System.Text;

namespace ApiPager.Core.FilterExpression
{
  public class UnaryLogicClause : AbstractExpressionClause
  {
    #region public enums
    public enum LogicTypeEnum
    {
      Not
    }
    #endregion

    #region public properties
    public LogicTypeEnum LogicType { get; set; }
    public AbstractExpressionClause Clause { get; set; }
    #endregion

    #region public methods
    public override void AddFieldValidationMessages(string[] filterColumns, StringBuilder errorMessagesBuilder)
    {
      Clause?.AddFieldValidationMessages(filterColumns, errorMessagesBuilder);
    }

    public override string ToString()
    {
      return "(" + LogicType + " " + Clause + ")";
    }
    #endregion
  }
}

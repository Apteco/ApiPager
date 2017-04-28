using System.Text;

namespace ApiPager.Core.FilterExpression
{
  public class BinaryLogicClause : AbstractExpressionClause
  {
    #region public enums
    public enum LogicTypeEnum
    {
      And,
      Or
    }
    #endregion

    #region public properties
    public LogicTypeEnum LogicType { get; set; }
    public AbstractExpressionClause LeftHandSide { get; set; }
    public AbstractExpressionClause RightHandSide { get; set; }
    #endregion

    #region public methods
    public override void AddFieldValidationMessages(string[] filterColumns, StringBuilder errorMessagesBuilder)
    {
        LeftHandSide?.AddFieldValidationMessages(filterColumns, errorMessagesBuilder);
        RightHandSide?.AddFieldValidationMessages(filterColumns, errorMessagesBuilder);
    }

    public override string ToString()
    {
      return "(" + LeftHandSide + " " + LogicType+" "+ RightHandSide + ")";
    }
    #endregion
  }
}

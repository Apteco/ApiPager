using System.Text;

namespace ApiPager.Core.FilterExpression
{
  public class ComparisonClause : AbstractExpressionClause
  {
    #region public enums
    public enum ComparisonTypeEnum
    {
      LessThan,
      LessThanOrEqualTo,
      EqualTo,
      GreaterThanOrEqualTo,
      GreaterThan,
      NotEqualTo,
      Like,
    }
    #endregion

    #region public properties
    public ComparisonTypeEnum ComparisonType { get; set; }
    public AbstractOperand LeftHandSide { get; set; }
    public AbstractOperand RightHandSide { get; set; }
    #endregion

    #region public methods
    public override void AddFieldValidationMessages(string[] filterColumns, StringBuilder errorMessagesBuilder)
    {
      LeftHandSide?.AddFieldValidationMessages(filterColumns, errorMessagesBuilder);
      RightHandSide?.AddFieldValidationMessages(filterColumns, errorMessagesBuilder);
    }

    public override string ToString()
    {
      return "(" + LeftHandSide + " " + ComparisonType + " " + RightHandSide + ")";
    }
    #endregion
  }
}

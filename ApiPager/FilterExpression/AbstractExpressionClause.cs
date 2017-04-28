using System;
using System.Text;

namespace ApiPager.Core.FilterExpression
{
  public abstract class AbstractExpressionClause
  {
    public abstract void AddFieldValidationMessages(string[] filterColumns, StringBuilder errorMessagesBuilder);
  }
}

using System.Text;

namespace ApiPager.Core.FilterExpression
{
  public abstract class AbstractOperand
  {
    public abstract void AddFieldValidationMessages(string[] filterColumns, StringBuilder errorMessagesBuilder);
  }
}

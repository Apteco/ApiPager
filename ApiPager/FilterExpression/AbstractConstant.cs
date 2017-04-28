using System.Text;

namespace ApiPager.Core.FilterExpression
{
  public abstract class AbstractConstant : AbstractOperand
  {
    public override void AddFieldValidationMessages(string[] filterColumns, StringBuilder errorMessagesBuilder)
    {
    }
  }
}
